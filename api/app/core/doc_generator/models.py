from __future__ import annotations

from typing import TYPE_CHECKING, Optional

if TYPE_CHECKING:
    from zipfile import ZipFile

from pydantic import BaseModel, ConfigDict

from app.api.schemas import DocInfo


class FeaturesRequest(BaseModel):
    name: str
    structure_sources: list[str]
    structure_docs: list[str]
    changed_sources: list[str]
    changed_docs: list[str]


class DocRequest(BaseModel):
    name: str
    structure_sources: list[str]
    changed_sources: list[str]
    feature: str
    current_doc: Optional[str]


class Feature(BaseModel):
    name: str
    children: list[Feature]


class GenerateDoc(BaseModel):
    info: DocInfo
    sources: ZipFile
    docs: Optional[ZipFile]

    model_config = ConfigDict(arbitrary_types_allowed=True)
