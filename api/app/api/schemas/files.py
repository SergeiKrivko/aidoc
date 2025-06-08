from __future__ import annotations

from typing import Optional, Any
from pydantic import BaseModel, Field


class Files(BaseModel):
    files: list[Optional[str]]


class Structure(BaseModel):
    name: str
    files: list[str]


class DocumentationFile(BaseModel):
    path: str
    position: int
    content: Any = None

    @staticmethod
    def paths(files: list[DocumentationFile]) -> list[str]:
        return [f.path for f in files] if files else []
