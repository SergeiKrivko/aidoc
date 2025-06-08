from typing import Optional

from fastapi import APIRouter, Response

from app.api import schemas
from app.services.template_svc import TemplateSvcDep

router = APIRouter()


@router.get("/templates/fill")
async def template_fill_handler(
    template_svc: TemplateSvcDep,
    name: str,
    github_repo: Optional[str] = None,
) -> Response:
    github_repo = github_repo or "https://github.com/SergeiKrivko/aidoc"
    return template_svc.fill(schemas.Info(name=name, github_repo=github_repo))
