from fastapi import FastAPI
from fastapi_mvp import Mvp
from fastapi_mvp.storage.mongo_storage import MongoSettings
from fastapi_mvp.storage.s3_storage import S3Settings

from app.api import routers
from app.api.exception_handler import endpoints_exception_handler
from app.settings.metrics_settings import get_metrics_settings
from app.settings.mongo_settings import get_mongo_settings
from app.settings.s3_settings import get_s3_settings


def create_app() -> FastAPI:
    app = FastAPI(
        title="AIDoc Agent API",
        description="API для генерации документации с помощью LLM",
        version="0.2.0",
        contact={
            "name": "AIDoc Support",
            "url": "https://github.com/SergeiKrivko/aidoc",
            "email": "contact@aleksei-orlov.ru",
        },
    )

    Mvp.setup(
        app,
        mongo=MongoSettings.model_validate(get_mongo_settings()),
        s3=S3Settings.model_validate(get_s3_settings()),
        metrics=get_metrics_settings(),
    )

    app.include_router(routers.documentation_router)

    app.exception_handler(Exception)(endpoints_exception_handler)

    return app
