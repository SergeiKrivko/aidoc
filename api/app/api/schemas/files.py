from typing import Optional

from pydantic import BaseModel


class Files(BaseModel):
    files: list[Optional[str]]


class Structure(BaseModel):
    name: str
    files: list[str]
