from fastapi import APIRouter, Response, HTTPException

from app.api import schemas
from app.services.agent_svc import OpenAISvcDep
from app.api.schemas.files import Structure
from app.services.uml_render import uml_render

router = APIRouter()


@router.post(path="/init", response_model=schemas.AgentResponseModel)
async def init_handler(
    ai_agent: OpenAISvcDep,
    agent_request: schemas.InitRequest,
) -> schemas.AgentResponseModel:
    result = await ai_agent.request(
        agent_request=agent_request,
        user_message=agent_request.model_dump_json(),
    )
    return result


@router.post(path="/features/init", response_model=schemas.AgentResponseModel)
async def features_init_handler(
    ai_agent: OpenAISvcDep,
    features_init: schemas.FeaturesInitRequest,
) -> schemas.AgentResponseModel:
    result = await ai_agent.features_init(features_init)
    return result


@router.post(path="/request", response_model=schemas.AgentResponseModel)
@router.post(path="/features/request", response_model=schemas.AgentResponseModel)
@router.post(path="/uml/request", response_model=schemas.AgentResponseModel)
async def request_handler(
    ai_agent: OpenAISvcDep,
    agent_request: schemas.AgentRequestModel,
) -> schemas.AgentResponseModel:
    result = await ai_agent.request(agent_request=agent_request)
    return result


@router.post(path="/uml_init", response_model=schemas.AgentResponseModel)
async def uml_init_handler(
    ai_agent: OpenAISvcDep,
    agent_request: schemas.UMLRequest,
) -> schemas.AgentResponseModel:
    result = await ai_agent.uml_init(agent_request=agent_request)
    return result


@router.post(path="/uml_render")
async def uml_render_handler(uml_code: schemas.UMLRenderRequest,
) -> Response:
    try:
        result = uml_render(uml_code.code)
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    else:
        return Response(content=result, media_type="image/png")
