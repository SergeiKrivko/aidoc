from pydantic_settings import SettingsConfigDict
from app.settings.env_settings import EnvSettings


class OpenAISettings(EnvSettings):
    model_config = SettingsConfigDict(
        env_prefix="OPENAI__",
    )

    token: str


def get_openai_settings() -> OpenAISettings:
    return OpenAISettings()
