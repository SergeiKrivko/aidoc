import io
import uuid
from functools import lru_cache
from typing import Annotated
from zipfile import ZIP_DEFLATED, ZipFile

from fastapi import Depends
from pydantic import HttpUrl

from app.api import schemas
from app.core.doc_generator import DocGenerator, get_doc_generator
from app.core.template_filler import TemplateFiller, get_template_filler


class DocumentationSvc:
    def __init__(
        self,
        generator: DocGenerator,
        template_filler: TemplateFiller,
    ) -> None:
        self._generator = generator
        self._template_filler = template_filler

    async def create_documentation(
        self,
        documentation_create: schemas.DocCreate,
    ) -> schemas.DocRead:
        documentation_id = uuid.uuid4()

        original_sources_url = HttpUrl("https://example.com/src")
        original_docs_url = HttpUrl("https://example.com/docs")

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

        return schemas.DocRead(
            id=documentation_id,
            info=documentation_create.info,
            status=schemas.DocCreationStatus.PROGRESS,
            original_sources_url=original_sources_url,
            original_docs_url=original_docs_url,
            result_docs_url=None,
        )


@lru_cache
def get_documentation_svc() -> DocumentationSvc:
    return DocumentationSvc(
        generator=get_doc_generator(),
        template_filler=get_template_filler(),
    )


DocumentationSvcDep = Annotated[DocumentationSvc, Depends(get_documentation_svc)]
