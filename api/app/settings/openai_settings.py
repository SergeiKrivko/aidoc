from pydantic_settings import SettingsConfigDict
from app.settings.env_settings import EnvSettings


class OpenAISettings(EnvSettings):
    model_config = SettingsConfigDict(
        env_prefix="OPENAI__",
    )

    token: str
    base_url: str | None = None


class DeepSeekSettings(EnvSettings):
    model_config = SettingsConfigDict(
        env_prefix="DEEPSEEK__",
    )

    token: str
    base_url: str 


def get_openai_settings() -> OpenAISettings:
    return OpenAISettings()

def get_deepseek_settings() -> DeepSeekSettings:
    return DeepSeekSettings()
