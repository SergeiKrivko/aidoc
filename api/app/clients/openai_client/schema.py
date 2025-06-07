from pydantic import BaseModel, RootModel, Field
from enum import Enum
from typing import Annotated, Optional, Any, TypedDict, Literal, Union
from datetime import datetime
from openai.types.chat import ChatCompletionMessageToolCall


class OpenAIModel(str, Enum):
    TURBO = "gpt-3.5-turbo"
    GPT4 = "gpt-4.1"
    GPT4_TURBO = "gpt-4-turbo-preview"
    GPT_4O = "gpt-4o-2024-11-20"
    DEEPSEEK = "deepseek-chat"
    DEEPSEEK_FAST = "deepseek-chat-fast"


class OpenAIRole(str, Enum):
    SYSTEM = "system"
    USER = "user"
    ASSIST = "assistant"
    TOOL = "tool"


class ParametersDesc(TypedDict):
    type: str
    format: str
    description: str


class ParametersModel(BaseModel):
    type: Annotated[
        str,
        Field(
            description="Тип схемы — всегда 'object' для объекта параметров функции."
        ),
    ] = "object"
    properties: dict[str, ParametersDesc] = Field(
        ...,
        examples={
            "date": {
                "type": "string",
                "format": "date-time",
                "description": "Дата, на которую нужно получить расписание. Формат ISO 8601, например 2025-04-20T00:00:00",
            }
        },
        description="Список параметров, которые принимает функция.",
    )
    required: list[str] = Field(
        ..., examples=["group", "date"], description="Обязательные параметры."
    )


class ToolFunction(BaseModel):
    """Описание вызываемой функции"""

    name: str = Field(..., description="Название функции")
    description: str = Field(..., description="Описание что делает функция")
    parameters: ParametersModel = Field(
        ..., description="Аргументы функции в формате JSON-строки"
    )


class ToolModel(BaseModel):
    type: str = Field(
        "function", description="Тип вызова, всегда 'function' для Function Calling"
    )
    function: ToolFunction = Field(...)


class ToolsModel(RootModel):
    root: list[ToolModel] = Field(...)


class ToolCallResult(BaseModel):
    role: Literal[OpenAIRole.TOOL] = Field(
        default=OpenAIRole.TOOL,
        frozen=True,
        description=f"В данном случае это поле всегда должно быть {OpenAIRole.TOOL.value}, чтобы указать, что это результат вызова функций",
    )
    tool_call_id: str = Field(
        ...,
        description="Уникальный идентификатор вызова",
        examples=["call_12345xyz", "call_abc6789"],
    )
    content: str = Field(
        ...,
        description="Результат выполнения функции в текстовом виде (gpt сам будет его анализировать)",
    )


class FunctionCall(BaseModel):
    """Аргументы вызова функции, возвращаемые моделью."""

    name: str = Field(
        ...,
        description="Название вызываемой функции",
        examples=["get_weather", "calculate_distance"],
    )
    arguments: str = Field(
        ...,
        description="Аргументы функции в виде JSON-строки",
        examples=['{"latitude": 48.8566, "longitude": 2.3522}'],
    )


class ToolCall(BaseModel):
    """Вызов инструмента (function calling) в ответе API."""

    id: str = Field(
        ...,
        description="Уникальный идентификатор вызова",
        examples=["call_12345xyz", "call_abc6789"],
    )
    type: str = Field(
        default="function",
        description="Тип вызова (в текущей версии API всегда 'function')",
        examples=["function"],
    )
    function: FunctionCall = Field(..., description="Данные о вызываемой функции")

    @staticmethod
    def vallidate_from_gpt_resp(gpt_resp: ChatCompletionMessageToolCall) -> "ToolCall":
        return ToolCall(
            id=gpt_resp.id,
            type=gpt_resp.type,
            function=FunctionCall(
                name=gpt_resp.function.name, arguments=gpt_resp.function.arguments
            ),
        )


class ToolCalls(BaseModel):
    """Модель для сообщения с вызовами инструментов."""

    tool_calls: Optional[list[ToolCall]] = Field(
        default=None,
        description="Список вызовов функций, возвращаемых моделью",
        examples=[
            [
                {
                    "id": "call_12345xyz",
                    "type": "function",
                    "function": {
                        "name": "get_weather",
                        "arguments": '{"latitude":48.8566,"longitude":2.3522}',
                    },
                }
            ]
        ],
    )

    @staticmethod
    def vallidate_from_gpt_resp(
        gpt_resp_call_list: list[ChatCompletionMessageToolCall],
    ) -> "ToolCalls":
        tool_calls = list()
        for call in gpt_resp_call_list:
            tool_calls.append(ToolCall.vallidate_from_gpt_resp(call))

        return ToolCalls(tool_calls=tool_calls)


class ToolCallsHistory(ToolCalls):
    role: Literal[OpenAIRole.ASSIST] = Field(
        default=OpenAIRole.ASSIST,
        frozen=True,
        description=f"В данном случае это поле всегда должно быть {OpenAIRole.ASSIST.value}, чтобы указать, что это вызовы, которые предложил сам gpt",
    )
    # опционально потому что когда гпт сообщает какие функции нужно вызвать content = null
    content: Optional[str] = Field(
        None,
        description="Результат выполнения функции в текстовом виде (gpt сам будет его анализировать)",
    )


class MessageModel(BaseModel):
    role: OpenAIRole = Field(..., examples=["user"])
    content: Optional[str] = Field(
        None, examples=["Какие у меня завтра пары?"], description="Содержимое сообщения"
    )


class MessagesModel(RootModel):
    root: list[Union[MessageModel, ToolCallResult, ToolCallsHistory]] = Field(...)

    def _create_date_model(self) -> MessageModel:
        return MessageModel(
            role=OpenAIRole.SYSTEM,
            content=f"current date in ISO 8601 FORMAT: {datetime.now().isoformat()}",
        )

    # обновляет (или добавляет, если отсутствует) контекст текущего времени
    # def _update_date(self) -> None:
    #     current_date_model = self._create_date_model()
    #     updated = False
    #     for i, msg in enumerate(self.root):
    #         if msg.role == OpenAIRole.SYSTEM and msg.content.startswith("current date"):
    #             self.root[i] = current_date_model
    #             updated = True

    #     if not updated:
    #         self.root.append(current_date_model)

    def add(self, msg: Union[MessageModel, ToolCallResult, ToolCallsHistory]) -> None:
        self.root.append(msg)


class OpenAIRequestModel(BaseModel):
    model: OpenAIModel = Field(..., examples="gpt-4")
    messages: MessagesModel = Field(...)
    user: Optional[str] = Field(
        None, examples=["Misha Kozin IU7-64B"], description="Имя участника диалога"
    )
    tools: Optional[ToolsModel] = Field(None)
    tool_choice: Optional[str] = Field("auto", description="какую функцию вызывать")

    def get_messages(self) -> list[dict[str, Any]]:
        return [msg.model_dump() for msg in self.messages.root]

    def get_tools(self) -> list[dict[str, Any]] | None:
        if self.tools is None:
            return None
        return [tool.model_dump() for tool in self.tools.root]
