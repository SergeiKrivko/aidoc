from pydantic import BaseModel, Field

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
