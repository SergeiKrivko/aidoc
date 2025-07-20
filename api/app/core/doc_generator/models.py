from __future__ import annotations

from typing import Optional
from zipfile import ZipFile  # noqa: TC003

from pydantic import BaseModel, ConfigDict

from app.api.schemas import DocInfo


class GenerateDoc(BaseModel):
    info: DocInfo
    sources: ZipFile
    docs: Optional[ZipFile]

    model_config = ConfigDict(arbitrary_types_allowed=True)
