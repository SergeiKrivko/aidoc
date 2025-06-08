from __future__ import annotations
from typing import Annotated, Optional

from pydantic import BaseModel, Field

from app.api.schemas import FeaturesInitRequest
from app.api.schemas.files import Structure, Files, DocumentationFile


class FeatureInitData(BaseModel):
    structure: Structure
    changed: Files
    documentation: Annotated[Optional[list[str]], Field(default_factory=list)]

    @classmethod
    def from_schema(cls, schema: FeaturesInitRequest) -> FeatureInitData:
        return cls(
            structure=schema.structure,
            changed=schema.changed,
            documentation=DocumentationFile.paths(schema.documentation),
        )
