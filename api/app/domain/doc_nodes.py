from __future__ import annotations

from pydantic import BaseModel


class DocFileNode(BaseModel):
    position: int


class DocStructure(BaseModel):
    files: list[DocFileNode]
    directories: list[DocDirectoryNode]


class DocDirectoryNode(BaseModel):
    name: str
    label: str
    position: int
    description: str = ""
    children: DocStructure
