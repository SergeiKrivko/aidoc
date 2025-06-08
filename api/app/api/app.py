from fastapi import FastAPI

from app.api import routers

from app.api.exception_handler import endpoints_exception_handler


def create_app() -> FastAPI:
    app = FastAPI(
        title="AIDoc Agent API",
        description="API для генерации документации с помощью LLM",
        version="0.1.0",
        contact={
            "name": "AIDoc Support",
            "url": "https://github.com/SergeiKrivko/aidoc",
            "email": "contact@aleksei-orlov.ru",
        },
    )

    app.include_router(routers.agent_router, prefix="/api/agent", tags=["agent"])
    app.include_router(routers.template_router, prefix="/api", tags=["template"])

    app.exception_handler(Exception)(endpoints_exception_handler)

    return app
