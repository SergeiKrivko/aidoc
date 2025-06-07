from typing import Optional

from fastapi import APIRouter, Query, Body

from app.api import schemas
from app.services.agent_svc import OpenAISvcDep

router = APIRouter()


@router.post(path="/init", response_model=schemas.AgentResponseModel)
async def agent_request(
    ai_agent: OpenAISvcDep,
    agent_request: schemas.InitRequest = Body(...),
) -> schemas.AgentResponseModel:

    result = await ai_agent.request(agent_request=agent_request, user_message=str(agent_request.model_dump()))

    return result


@router.post(path="/request", response_model=schemas.AgentResponseModel)
async def agent_request(
    ai_agent: OpenAISvcDep,
    agent_request: schemas.AgentRequestModel = Body(...)
    ) -> schemas.AgentResponseModel:
    
    result = await ai_agent.request(agent_request=agent_request)
    
    return result
