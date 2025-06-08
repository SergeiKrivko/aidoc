from typing import Annotated, Optional

from pydantic import Field

from pydantic import BaseModel

from app.api.schemas.files import Structure, Files, DocumentationFile


class FeaturesInitRequest(BaseModel):
    structure: Structure
    changed: Files
    documentation: Annotated[
        Optional[list[DocumentationFile]], Field(default_factory=list)
    ]
