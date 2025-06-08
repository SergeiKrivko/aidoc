from fastapi import APIRouter, Response

from app.api import schemas
from app.services.template_svc import TemplateSvcDep

router = APIRouter()


@router.post("/templates/fill")
async def template_fill_handler(
    template_svc: TemplateSvcDep,
    info: schemas.Info,
) -> Response:
    return template_svc.fill(info)
