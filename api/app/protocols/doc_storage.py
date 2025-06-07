from __future__ import annotations
from typing import Protocol

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


class IDocStorage(Protocol):
    async def load_structure(self) -> list[DocStructure]: ...
    async def load_file(self, root_relative_path: str) -> str: ...
    async def save_file(self, root_relative_path: str, data: str) -> None: ...
