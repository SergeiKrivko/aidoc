from enum import StrEnum
from typing import Optional
from uuid import UUID
from zipfile import ZipFile

from pydantic import BaseModel, ConfigDict, HttpUrl


class DocumentationCreationStatus(StrEnum):
    PROGRESS = "progress"
    DONE = "done"
    FAILED = "failed"


class DocumentationBase(BaseModel):
    application_name: str
    changed_sources: list[str]
    changed_docs: list[str]


class DocumentationCreate(DocumentationBase):
    sources: ZipFile
    docs: Optional[ZipFile]

    model_config = ConfigDict(arbitrary_types_allowed=True)


class DocumentationRead(DocumentationBase):
    id: UUID
    status: DocumentationCreationStatus
    original_sources_url: HttpUrl
    original_docs_url: HttpUrl
    result_docs_url: Optional[HttpUrl]


class DocumentationCreateResponse(BaseModel):
    data: DocumentationRead
    detail: str = "Documentation generation started."
