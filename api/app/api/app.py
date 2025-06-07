from fastapi import FastAPI


def create_app() -> FastAPI:
    app = FastAPI(
        title="AIDoc API",
        description="API для генерации документации с помощью LLM",
        version="0.1.0",
        contact={
            "name": "AIDoc Support",
            "url": "https://github.com/SergeiKrivko/aidoc",
            "email": "contact@aleksei-orlov.ru",
        },
    )

    # app.include_router(your_package.router)

    return app
