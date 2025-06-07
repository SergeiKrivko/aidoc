import os
from functools import lru_cache

from pydantic import Field
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
