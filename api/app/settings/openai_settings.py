from functools import lru_cache

from pydantic_settings import SettingsConfigDict
from app.settings.env_settings import EnvSettings


class OpenAISettings(EnvSettings):
    model_config = SettingsConfigDict(
        env_prefix="OPENAI__",
    )

    token: str
    max_message_size: int = 50000


@lru_cache
def get_openai_settings() -> OpenAISettings:
    return OpenAISettings()
