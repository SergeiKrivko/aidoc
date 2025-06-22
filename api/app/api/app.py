from fastapi import FastAPI
from fastapi_mvp import Mvp

from app.api import routers
from app.api.exception_handler import endpoints_exception_handler
from app.settings.metrics_settings import get_metrics_settings


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

    Mvp.setup(app, metrics=get_metrics_settings())

    app.include_router(routers.documentation_router)

    app.exception_handler(Exception)(endpoints_exception_handler)

    return app
