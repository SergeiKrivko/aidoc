from fastapi import APIRouter, Query, Body

from agent.schema import *
from agent.service import ai_agent

router = APIRouter()


@router.post(path="/request", response_model=AgentResponseModel)
async def agent_request(
    init: bool = Query(...),
    agent_request: AgentRequestModel | InitRequest = Body(...)
    ) -> AgentResponseModel:
    
    if init:
        result = await ai_agent.request(agent_request=agent_request, user_message=str(agent_request.model_dump()))
    else:
        result = await ai_agent.request(agent_request=agent_request)
    
    return result
