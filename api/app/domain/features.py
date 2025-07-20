from __future__ import annotations

from pydantic import BaseModel


class Feature(BaseModel):
    name: str
    path: list[str]

    @property
    def doc_path(self) -> str:
        parent_path = "/".join(self.path)
        return f"{parent_path}/{self.name}.mdx"
