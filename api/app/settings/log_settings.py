from functools import lru_cache

from fastapi_mvp.settings import AppSettings
from pydantic_settings import SettingsConfigDict


class LogSettings(AppSettings):
    model_config = SettingsConfigDict(
        env_prefix="LOG__",
    )

    level: str = "INFO"
    folder_path: str = "logs"


@lru_cache
def get_log_settings() -> LogSettings:
    return LogSettings()
