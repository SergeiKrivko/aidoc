from openai import AsyncOpenAI

from app.clients.openai_client.schema import OpenAIRequestModel
from app.core import logger


class OpenAIClient:
    def __init__(self, api_key: str, base_url: str | None = None):
        self._base_url = base_url
        self._api_key = api_key
        self._openai_client = AsyncOpenAI(
            api_key=self._api_key, base_url=self._base_url
        )
        self.client = self._openai_client

    async def request(self, request_model: OpenAIRequestModel):
        model = request_model.model
        messages = request_model.get_messages()
        tools = request_model.get_tools()
        tool_choice = request_model.tool_choice

        logger.log_json(request_model.model_dump())

        response = await self.client.chat.completions.create(
            model=model,
            messages=messages,
            tools=tools,
            tool_choice=tool_choice,
        )

        return response
