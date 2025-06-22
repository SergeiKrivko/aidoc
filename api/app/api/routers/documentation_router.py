import io
from typing import Annotated, Optional
from zipfile import ZipFile

from fastapi import APIRouter, Form, UploadFile

from app.api import schemas
from app.services import DocumentationSvcDep

router = APIRouter()


DocInfoForm = Annotated[
    str,
    Form(description="Информация для генерации документации"),
]
SourcesFile = Annotated[
    UploadFile,
    Form(description="Архив исходных файлов"),
]
DocsFile = Annotated[
    Optional[UploadFile],
    Form(description="Архив файлов документации"),
]


@router.post(
    "/api/v1/documentation",
    summary="Поставить задачу на генерацию документации",
    description=(
        "Ставится задача на генерацию, на фоне происходит общение с гпт. "
        "Для получения результата надо поллить ручку GET `/api/v1/documentation/{id}`. "
        "Информация для генерации должна быть в формате JSON "
        "со структурой как в возвращаемой модели в поле `info`."
    ),
    tags=["documentation"],
    response_model=schemas.DocCreateResponse,
)
async def create_documentation_handler(
    documentation_svc: DocumentationSvcDep,
    info: DocInfoForm,
    sources: SourcesFile,
    docs: DocsFile = None,
) -> schemas.DocCreateResponse:
    sources_zip = ZipFile(io.BytesIO(await sources.read()))
    docs_zip = ZipFile(io.BytesIO(await docs.read())) if docs else None

    create = schemas.DocCreate(
        info=schemas.DocInfo.model_validate_json(info),
        sources=sources_zip,
        docs=docs_zip,
    )

    read = await documentation_svc.create_documentation(create)

    return schemas.DocCreateResponse(data=read)
