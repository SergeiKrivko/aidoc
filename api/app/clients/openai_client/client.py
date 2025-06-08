from functools import lru_cache

from openai import AsyncOpenAI

from app.clients.openai_client.schema import OpenAIRequestModel
from app.core import logger
from app.settings.openai_settings import get_openai_settings, OpenAISettings, get_deepseek_settings, DeepSeekSettings


class OpenAIClient:
    def __init__(self, settings: OpenAISettings | DeepSeekSettings):
        self._api_key = settings.token
        self._openai_client = AsyncOpenAI(
            api_key=self._api_key,
            base_url=settings.base_url
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


@lru_cache
def get_openai_client() -> OpenAIClient:
    return OpenAIClient(get_openai_settings())


@lru_cache
def get_deepseek_client() -> OpenAIClient:
    return OpenAIClient(get_deepseek_settings())