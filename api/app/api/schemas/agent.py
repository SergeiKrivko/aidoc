from pydantic import BaseModel, Field
from typing import Optional

from app.clients.openai_client import MessagesModel


class AgentRequestModel(BaseModel):
    messages: MessagesModel = Field(...)


class AgentResponseModel(BaseModel):
    messages: MessagesModel = Field(
        ..., description="messages от gpt с историей сообщений и его ответов"
    )


class FunctionResultsModel(BaseModel):
    messages: MessagesModel = Field(
        ...,
    )
    function_result: str = Field(
        ...,
        description="Результат выполнения функции в текстовом виде (gpt сам будет его анализировать)",
    )

class Files(BaseModel):
    files: list[Optional[str]] = Field(...)


class Structure(BaseModel):
    name: str = Field(...)
    files: list[str] = Field(...)


class InitRequest(BaseModel):
    structure: Structure = Field(...)
    changed: Files = Field(...)
    feature: str = Field(...)
    current_doc: Optional[str] = Field(None)
