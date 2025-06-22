from functools import lru_cache

from pydantic_settings import SettingsConfigDict

from app.settings.env_settings import EnvSettings


class MongoSettings(EnvSettings):
    model_config = SettingsConfigDict(env_prefix="MONGO__")

    name: str
    user: str
    password: str
    host: str
    port: int


@lru_cache
def get_mongo_settings() -> MongoSettings:
    return MongoSettings()
