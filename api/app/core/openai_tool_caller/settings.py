from functools import lru_cache

from fastapi_mvp.settings import AppSettings
from pydantic_settings import SettingsConfigDict


class OpenAIToolCallerSettings(AppSettings):
    model_config = SettingsConfigDict(env_prefix="OPENAI_TOOL_CALLER__")

    base_url: str


@lru_cache
def get_openai_tool_caller_settings() -> OpenAIToolCallerSettings:
    return OpenAIToolCallerSettings()
