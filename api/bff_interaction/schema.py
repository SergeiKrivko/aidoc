from pydantic import BaseModel, Field
from typing import Type, Any, Optional
import json

from openai_api.schema import *


class Pair(BaseModel):
    teachers: Optional[list[str]] = None
    groups: Optional[list[str]] = None
    startTime: datetime
    endTime: datetime
    rooms: Optional[list[str]] = None
    discipline: Optional[str] = None
    actType: Optional[str] = None


class MergedPair(BaseModel):
    startTime: datetime
    endTime: datetime
    actType: Optional[str] = None
    discipline: Optional[str] = None
    rooms: Optional[list[str]] = None
    convenience: float = Field(..., description="Удобство - насколько удобно (от 0 до 1) исходя из всех факторов подойти к преподаветелю в это время")
    collisions: Optional[list[Pair]] = None
    waitTime: Optional[str] = None  # Assuming date-span format is string


class ResponseFormat(BaseModel):
    function_calling_needed: bool = Field(...)
    clarification_needed: bool = Field(...)
    message: str = Field(...)
    schedule: Optional[dict[str, Any]] = Field(None)


def save_schema_to_json(model: Type[BaseModel], file_path: str) -> None:
    """Сохраняет схему модели в JSON файл."""
    schema = model.model_json_schema()
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(schema, f, ensure_ascii=False, indent=4)
