from pydantic import BaseModel

from app.api.schemas.files import Structure, Files


class FeaturesInitRequest(BaseModel):
    structure: Structure
    changed: Files
