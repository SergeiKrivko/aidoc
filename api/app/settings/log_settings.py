from pydantic_settings import SettingsConfigDict

from app.settings.env_settings import EnvSettings


class LogSettigns(EnvSettings):
    model_config = SettingsConfigDict(
        env_prefix="LOG__",
    )

    level: str = "INFO"
    folder_path: str = "logs"


def get_log_settings() -> LogSettigns:
    return LogSettigns()
