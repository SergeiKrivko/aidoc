from fastapi import Depends
from functools import lru_cache
from typing import Optional, Annotated

from app.clients.openai_client import (
    MessageModel,
    OpenAIRole,
    ToolCalls,
    ToolCallsHistory,
)
from app.clients.openai_client.client import get_openai_client
from app.clients.openai_client.schema import OpenAIRequestModel, OpenAIModel
from bff_interaction.client import get_bff_client
from app.api import schemas

import os


class AIAgentService:
    def __init__(self):
        self.openai_client = get_openai_client()
        self.data_client = get_bff_client()

    async def request(
        self,
        agent_request: schemas.AgentRequestModel | schemas.InitRequest,
        user_message: Optional[str] = None,
    ) -> schemas.AgentResponseModel:
        if user_message is None and agent_request is None:
            raise ValueError(
                "user_message and messages cannot be None at the same time!"
            )

        if user_message:
            messages = self.data_client.get_context()
            print("Messages:", messages)
            messages.add(MessageModel(role=OpenAIRole.USER, content=user_message))
        else:
            messages = agent_request.messages

        tools = self.data_client.get_tools()
        print("Tools:", tools)

        request_model = OpenAIRequestModel(
            model=OpenAIModel.GPT_4O, messages=messages, tools=tools
        )

        result = await self.openai_client.request(request_model)

        # сохраняем в messages информацию, которую нам предоставил gpt о том, какие функции нужно вызвать
        if result.choices[0].message.tool_calls is not None:
            tool_calls = ToolCalls.vallidate_from_gpt_resp(
                result.choices[0].message.tool_calls
            )
            messages.add(
                ToolCallsHistory(
                    tool_calls=tool_calls.tool_calls,
                    content=result.choices[0].message.content,
                )
            )
        else:
            messages.add(
                ToolCallsHistory(
                    tool_calls=None, content=result.choices[0].message.content
                )
            )

        return schemas.AgentResponseModel(messages=messages)


@lru_cache
def get_openai_service() -> AIAgentService:
    return AIAgentService()


OpenAISvcDep = Annotated[AIAgentService, Depends(get_openai_service)]
