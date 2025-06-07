from typing import Optional

from fastapi import APIRouter, Query, Body

from app.api import schemas
from app.services.agent_svc import OpenAISvcDep

router = APIRouter()


@router.post(path="/request", response_model=schemas.AgentResponseModel)
async def agent_request(
    ai_agent: OpenAISvcDep,
    user_message: Optional[str] = Query(None),
    agent_request: Optional[schemas.AgentRequestModel] = Body(None),
) -> schemas.AgentResponseModel:

    result = await ai_agent.request(
        agent_request=agent_request, user_message=user_message
    )

    return result
