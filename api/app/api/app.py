import os

from fastapi_mvp import FastAPIMvp
from fastapi_mvp.metrics import MetricsSettings
from fastapi_mvp.settings import LoadEnvSettings
from fastapi_mvp.storage.mongo_storage import MongoSettings
from fastapi_mvp.storage.s3_storage import S3Settings

from app.api import routers
from app.api.exception_handler import endpoints_exception_handler


def create_app() -> FastAPIMvp:
    env = os.getenv("ENV")
    if env is None:
        err = "Set ENV to 'local' or 'production' to run this app"
        raise RuntimeError(err)

    FastAPIMvp.prepare_env(LoadEnvSettings(env=env))

    app = FastAPIMvp(
        title="AIDoc API",
        description="API для генерации документации с помощью LLM",
        version="0.2.0",
        contact={
            "name": "AIDoc Support",
            "url": "https://github.com/SergeiKrivko/aidoc",
            "email": "contact@aleksei-orlov.ru",
        },
        mongo=MongoSettings(
            name="mongo",
            port=27017,
        ),
        s3=S3Settings(
            bucket="aidoc",
            host="s3.cloud.ru",
            region="ru-central-1",
        ),
        metrics=MetricsSettings(),
    )

    app.include_router(routers.documentation_router)

    app.exception_handler(Exception)(endpoints_exception_handler)

    return app
