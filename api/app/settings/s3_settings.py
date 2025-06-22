from functools import lru_cache

from pydantic_settings import SettingsConfigDict

from app.settings.env_settings import EnvSettings


class S3Settings(EnvSettings):
    model_config = SettingsConfigDict(env_prefix="S3__")

    bucket: str
    host: str
    protocol: str
    region: str
    aws_access_key_id: str
    aws_secret_access_key: str


@lru_cache
def get_s3_settings() -> S3Settings:
    return S3Settings()
