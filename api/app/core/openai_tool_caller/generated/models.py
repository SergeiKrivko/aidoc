from typing import Annotated, Optional

from pydantic import BaseModel, Field


class GetFileRequest(BaseModel):
    path: Annotated[str, Field(description="Путь к файлу от корневой папки")]


class GetFileResponse(BaseModel):
    content: Annotated[
        Optional[str],
        Field(
            description=(
                "Содержимое файла. None если файл не найден, "
                "либо если это не файл с исходным кодом."
            ),
        ),
    ]


class GenerateFeaturesRequest(BaseModel):
    name: str
    structure_sources: list[str]
    structure_docs: list[str]
    changed_sources: list[str]
    changed_docs: list[str]


class GenerateDocsRequest(BaseModel):
    name: str
    structure_sources: list[str]
    changed_sources: list[str]
    feature: str
    current_doc: Optional[str]
