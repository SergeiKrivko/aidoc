from functools import lru_cache
from typing import Annotated

from fastapi import Depends, Response

from app.api import schemas
from app.core.template_filler import TemplateFiller, get_template_filler


class TemplateSvc:
    def __init__(self, template_filler: TemplateFiller):
        self.template_filler = template_filler

    def fill(self, info: schemas.Info) -> Response:
        archive_bytes = self.template_filler.fill(info)
        return Response(
            content=archive_bytes,
            media_type="application/octet-stream",
            headers={"Content-Disposition": "attachment; filename=static.zip"},
        )


@lru_cache
def get_template_svc() -> TemplateSvc:
    return TemplateSvc(get_template_filler())


TemplateSvcDep = Annotated[TemplateSvc, Depends(get_template_svc)]
