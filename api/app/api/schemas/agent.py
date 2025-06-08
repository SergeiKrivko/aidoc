from pydantic import BaseModel, Field, RootModel
from typing import Optional

from app.api.schemas.files import Structure, Files
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


class InitRequest(BaseModel):
    structure: Structure = Field(...)
    changed: Files = Field(...)
    feature: str = Field(...)
    current_doc: Optional[str] = Field(None)


class UMLRequest(RootModel[Structure]):
    pass

class UMLRenderRequest(BaseModel):
    code: str = Field(...)