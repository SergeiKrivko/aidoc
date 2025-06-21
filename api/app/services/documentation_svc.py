import uuid
from functools import lru_cache
from typing import Annotated

from fastapi import Depends
from pydantic import HttpUrl

from app.api import schemas
from app.core.doc_archiver import DocArchiver, get_doc_archiver
from app.core.doc_generator import DocGenerator, get_doc_generator


class DocumentationSvc:
    def __init__(
        self,
        generator: DocGenerator,
        archiver: DocArchiver,
    ) -> None:
        self._generator = generator
        self._archiver = archiver

    async def create_documentation(
        self,
        documentation_create: schemas.DocumentationCreate,
    ) -> schemas.DocumentationRead:
        documentation_id = uuid.uuid4()

        original_sources_url = HttpUrl("https://example.com/src")
        original_docs_url = HttpUrl("https://example.com/docs")

        # todo пока синхронно, чтобы было проще тестировать
        docs = await self._generator.generate(documentation_create)
        docs_archive = await self._archiver.archive(docs)
        # todo save result to s3
        with open("myarchive.zip", "wb") as f:  # noqa
            f.write(docs_archive)

        return schemas.DocumentationRead(
            id=documentation_id,
            application_name=documentation_create.application_name,
            status=schemas.DocumentationCreationStatus.PROGRESS,
            changed_sources=documentation_create.changed_sources,
            changed_docs=documentation_create.changed_docs,
            original_sources_url=original_sources_url,
            original_docs_url=original_docs_url,
            result_docs_url=None,
        )


@lru_cache
def get_documentation_svc() -> DocumentationSvc:
    return DocumentationSvc(
        generator=get_doc_generator(),
        archiver=get_doc_archiver(),
    )


DocumentationSvcDep = Annotated[DocumentationSvc, Depends(get_documentation_svc)]
