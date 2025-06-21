from __future__ import annotations

from typing import Optional

from pydantic import BaseModel


class DocGeneratedFile(BaseModel):
    path: str
    content: str
    position: Optional[int] = None
