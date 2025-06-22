from functools import lru_cache

from pydantic_settings import SettingsConfigDict

from app.settings.env_settings import EnvSettings


class OpenAIToolCallerSettings(EnvSettings):
    model_config = SettingsConfigDict(env_prefix="OPENAI_TOOL_CALLER__")

    base_url: str


@lru_cache
def get_openai_tool_caller_settings() -> OpenAIToolCallerSettings:
    return OpenAIToolCallerSettings()
