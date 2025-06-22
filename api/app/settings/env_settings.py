import os

from pydantic_settings import BaseSettings, SettingsConfigDict

ENV = os.getenv("ENV", "local")
ENVS_DIR = "envs"
ENV_FILE = f"{ENVS_DIR}/{ENV}.env"
COMMON_ENV_FILE = f"{ENVS_DIR}/common.env"


class EnvSettings(BaseSettings):
    model_config = SettingsConfigDict(
        env_file=(COMMON_ENV_FILE, ENV_FILE),
        env_nested_delimiter="__",
        extra="ignore",
    )

    def __hash__(self) -> int:
        """Костыль, чтобы можно было использовать lru_cache."""
        return hash(self.model_dump_json())
