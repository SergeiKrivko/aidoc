from __future__ import annotations
from typing import Protocol
from pydantic import BaseModel


class StructureNode(BaseModel):
    is_directory: bool
    name: str
    root_relative_path: str
    children: list[StructureNode]


class ISourceLoader(Protocol):
    async def load_structure(self) -> list[StructureNode]: ...
    async def load_file(self, root_relative_path: str) -> str: ...
    async def get_diff_filenames(self) -> list[str]: ...
    async def get_diff(self, root_relative_path: str) -> str: ...
