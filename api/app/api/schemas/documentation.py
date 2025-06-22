from enum import StrEnum
from typing import Optional
from uuid import UUID
from zipfile import ZipFile

from pydantic import BaseModel, ConfigDict, HttpUrl

from app.api.schemas.info import AppInfo


class DocCreationStatus(StrEnum):
    PROGRESS = "progress"
    DONE = "done"
    FAILED = "failed"


class DocInfo(BaseModel):
    application_info: AppInfo
    changed_sources: list[str]
    changed_docs: list[str]


class DocCreate(BaseModel):
    info: DocInfo
    sources: ZipFile
    docs: Optional[ZipFile]

    model_config = ConfigDict(arbitrary_types_allowed=True)


class DocRead(BaseModel):
    id: UUID
    status: DocCreationStatus
    info: DocInfo
    original_sources_url: HttpUrl
    original_docs_url: HttpUrl
    result_docs_url: Optional[HttpUrl]


class DocCreateResponse(BaseModel):
    data: DocRead
    detail: str = "Documentation generation started."
