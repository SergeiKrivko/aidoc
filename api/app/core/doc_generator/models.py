from __future__ import annotations

from typing import Optional

from pydantic import BaseModel


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
