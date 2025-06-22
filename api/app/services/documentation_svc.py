import io
import uuid
from datetime import timedelta
from functools import lru_cache
from typing import TYPE_CHECKING, Annotated, Optional
from zipfile import ZIP_DEFLATED, ZipFile

from fastapi import BackgroundTasks, Depends
from fastapi_mvp import MvpDep
from fastapi_mvp.storage.mongo_storage import MongoStorage
from fastapi_mvp.storage.s3_storage import S3Storage
from loguru import logger

if TYPE_CHECKING:
    from pydantic import HttpUrl

from app.api import schemas
from app.api.exception_handler import NotFoundError
from app.core.doc_generator import DocGenerator, GenerateDoc, get_doc_generator
from app.core.template_filler import TemplateFiller, get_template_filler


class DocumentationSvc:
    def __init__(
        self,
        generator: DocGenerator,
        template_filler: TemplateFiller,
        mongo_storage: MongoStorage,
        s3_storage: S3Storage,
    ) -> None:
        self._generator = generator
        self._template_filler = template_filler
        self._db = mongo_storage
        self._s3 = s3_storage
        self._url_expires_in = timedelta(days=1)

    async def get_doc(self, doc_id: uuid.UUID) -> schemas.DocRead:
        doc = await self._db.load(f"doc/{doc_id}", schemas.DocRead)
        if not doc:
            err = f"Documentation not found: {doc_id}"
            raise NotFoundError(err)

        return doc

    async def create_doc_task(
        self,
        doc_create: schemas.DocCreate,
        bt: BackgroundTasks,
    ) -> schemas.DocRead:
        """
        Постановки задачи на асинхронную генерацию документации.
        Сохранение данных в бд и s3.
        :param doc_create: Исходные данные для генерации.
        :param bt: Экземпляр BackgroundTasks FastAPI.
        :return: Данные о задаче на генерацию.
        """
        doc_id = uuid.uuid4()

        sources_url = await self._s3.save(
            key=f"original_sources/{doc_id}",
            data=doc_create.sources,
            expires_in=self._url_expires_in,
        )

        docs_url: Optional[HttpUrl] = None
        if doc_create.docs:
            docs_url = await self._s3.save(
                key=f"original_docs/{doc_id}",
                data=doc_create.sources,
                expires_in=self._url_expires_in,
            )

        doc = schemas.DocRead(
            id=doc_id,
            info=doc_create.info,
            status=schemas.DocCreationStatus.PROGRESS,
            original_sources_url=sources_url,
            original_docs_url=docs_url,
            result_docs_url=None,
            error_description=None,
        )
        saved_doc = await self._db.save(f"doc/{doc_id}", doc)

        # Запускаем генерацию на фоне
        bt.add_task(self.create_doc, doc_id, doc_create)
        return saved_doc

    async def create_doc(
        self,
        doc_id: uuid.UUID,
        doc_create: schemas.DocCreate,
    ) -> None:
        """
        Асинхронная генерация документации.
        :param doc_id: Идентификатор задачи на генерацию.
        :param doc_create: Исходные данные для генерации.
        :return:
        """
        result_url: Optional[HttpUrl] = None
        status: schemas.DocCreationStatus
        error_description: Optional[str] = None
        try:
            doc_bytes = await self._generate_doc(doc_create)
            result_url = await self._s3.save(
                key=f"results/{doc_id}",
                data=doc_bytes,
                expires_in=self._url_expires_in,
            )
            status = schemas.DocCreationStatus.DONE
        except Exception as e:  # noqa: BLE001
            error_description = f"Documentation generation failed: {e}"
            logger.error(error_description)
            status = schemas.DocCreationStatus.FAILED

        doc = await self._db.load(f"doc/{doc_id}", schemas.DocRead)
        doc.result_docs_url = result_url
        doc.status = status
        doc.error_description = error_description
        await self._db.save(f"doc/{doc_id}", doc)

    async def _generate_doc(self, doc_create: schemas.DocCreate) -> bytes:
        buffer = io.BytesIO()
        original_sources = ZipFile(io.BytesIO(doc_create.sources))
        result_docs = ZipFile(buffer, mode="w", compression=ZIP_DEFLATED)

        # Если еще нет доки, нужно закинуть в архив статику
        if doc_create.docs is None:
            await self._template_filler.fill(
                dst=result_docs,
                info=doc_create.info.application_info,
            )
            original_docs = None
        else:
            original_docs = ZipFile(io.BytesIO(doc_create.docs))

        # Пишем документацию в /docs
        await self._generator.generate(
            dst=result_docs,
            data=GenerateDoc(
                info=doc_create.info,
                sources=original_sources,
                docs=original_docs,
            ),
        )

        result_docs.close()
        original_sources.close()
        if original_docs:
            original_docs.close()

        return buffer.getvalue()


@lru_cache
def get_documentation_svc(mvp: MvpDep) -> DocumentationSvc:
    return DocumentationSvc(
        generator=get_doc_generator(),
        template_filler=get_template_filler(),
        mongo_storage=mvp.mongo(),
        s3_storage=mvp.s3(),
    )


DocumentationSvcDep = Annotated[DocumentationSvc, Depends(get_documentation_svc)]
