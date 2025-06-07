from fastapi import Depends
from functools import lru_cache
from typing import Optional, Annotated

from app.clients.openai_client import (
    OpenAIClient,
    MessageModel,
    OpenAIRole,
    ToolCalls,
    ToolCallsHistory,
)
from app.clients.openai_client.schema import OpenAIRequestModel, OpenAIModel
from bff_interaction.client import Client as DataClient
from app.api import schemas
from app.settings import OpenAISettings, bff_settings, openai_settings

import os


class AIAgentService:
    def __init__(self, settings: OpenAISettings):
        self.openai_client = OpenAIClient(api_key=settings.token)
        self.data_client = DataClient(bff_settings.get_bff_settings())

    async def request(
        self,
        agent_request: Optional[schemas.AgentRequestModel],
        user_message: Optional[str],
    ) -> schemas.AgentResponseModel:
        if user_message is None and agent_request is None:
            raise ValueError(
                "user_message and messages cannot be None at the same time!"
            )

        if agent_request is None:
            messages = self.data_client.get_context()
            # добавляем сообщение пользователя и информацию о его группе обучения
            # messages.add(MessageModel(role=OpenAIRole.USER, content=self._get_user_data_str(group, university)))
        else:
            messages = agent_request.messages
            # messages._update_date()

        if user_message is not None:
            messages.add(MessageModel(role=OpenAIRole.USER, content=user_message))

        tools = self.data_client.get_tools()

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
    return AIAgentService(openai_settings.get_openai_settings())


OpenAISvcDep = Annotated[AIAgentService, Depends(get_openai_service)]
