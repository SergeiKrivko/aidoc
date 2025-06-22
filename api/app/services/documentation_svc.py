import io
import uuid
from functools import lru_cache
from typing import Annotated
from zipfile import ZIP_DEFLATED, ZipFile

from fastapi import Depends
from fastapi_mvp import MvpDep
from fastapi_mvp.storage.mongo_storage import MongoStorage
from pydantic import HttpUrl

from app.api import schemas
from app.api.exception_handler import NotFoundError
from app.core.doc_generator import DocGenerator, get_doc_generator
from app.core.template_filler import TemplateFiller, get_template_filler


class DocumentationSvc:
    def __init__(
        self,
        generator: DocGenerator,
        template_filler: TemplateFiller,
        mongo_storage: MongoStorage,
    ) -> None:
        self._generator = generator
        self._template_filler = template_filler
        self._db = mongo_storage

    async def get_documentation(self, doc_id: uuid.UUID) -> schemas.DocRead:
        doc = await self._db.load(f"doc/{doc_id}", schemas.DocRead)
        if not doc:
            err = f"Documentation not found: {doc_id}"
            raise NotFoundError(err)

        return doc

    async def create_documentation(
        self,
        documentation_create: schemas.DocCreate,
    ) -> schemas.DocRead:
        doc_id = uuid.uuid4()

        doc = schemas.DocRead(
            id=doc_id,
            info=documentation_create.info,
            status=schemas.DocCreationStatus.PROGRESS,
            original_sources_url=HttpUrl("https://example.com/src"),
            original_docs_url=HttpUrl("https://example.com/docs"),
            result_docs_url=None,
        )
        await self._db.save(f"doc/{doc_id}", doc)

        # todo пока синхронно, чтобы было проще тестировать

        buffer = io.BytesIO()
        with ZipFile(buffer, mode="w", compression=ZIP_DEFLATED) as docs_archive:
            # Если еще нет доки, нужно закинуть в архив статику
            if documentation_create.docs is None:
                await self._template_filler.fill(
                    dst=docs_archive,
                    info=documentation_create.info.application_info,
                )
            # Пишем документацию в /docs
            await self._generator.generate(
                dst=docs_archive,
                data=documentation_create,
            )

        # todo save result to s3
        with open("myarchive.zip", "wb") as f:  # noqa
            f.write(buffer.getvalue())

        return doc


@lru_cache
def get_documentation_svc(mvp: MvpDep) -> DocumentationSvc:
    return DocumentationSvc(
        generator=get_doc_generator(),
        template_filler=get_template_filler(),
        mongo_storage=mvp.mongo(),
    )


DocumentationSvcDep = Annotated[DocumentationSvc, Depends(get_documentation_svc)]
