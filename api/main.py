from fastapi import FastAPI

from agent.router import router as agent_router
from utils.exceptions import endpoints_exception_handler

app = FastAPI(title="AIAgent")

app.include_router(agent_router, prefix="/api/agent", tags=["agent"])

app.exception_handler(Exception)(endpoints_exception_handler)
