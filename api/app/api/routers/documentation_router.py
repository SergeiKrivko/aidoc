import io
from typing import Annotated, Optional
from zipfile import ZipFile

from fastapi import APIRouter, Form, UploadFile

from app.api import schemas
from app.services import DocumentationSvcDep

router = APIRouter()


ApplicationNameForm = Annotated[
    str,
    Form(description="Название приложения"),
]
ChangedSourcesForm = Annotated[
    list[str],
    Form(
        description="Список изменившихся исходных файлов",
        default_factory=list,
    ),
]
ChangedDocsForm = Annotated[
    list[str],
    Form(
        description="Список изменившихся файлов документации",
        default_factory=list,
    ),
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
        "Для получения результата надо поллить ручку GET `/api/v1/documentation/{id}`."
    ),
    tags=["documentation"],
    response_model=schemas.DocumentationCreateResponse,
)
async def create_documentation_handler(
    documentation_svc: DocumentationSvcDep,
    application_name: ApplicationNameForm,
    changed_sources: ChangedSourcesForm,
    changed_docs: ChangedDocsForm,
    sources: SourcesFile,
    docs: DocsFile = None,
) -> schemas.DocumentationCreateResponse:
    sources_zip = ZipFile(io.BytesIO(await sources.read()))
    docs_zip = ZipFile(io.BytesIO(await docs.read())) if docs else None

    create = schemas.DocumentationCreate(
        application_name=application_name,
        changed_sources=changed_sources,
        changed_docs=changed_docs,
        sources=sources_zip,
        docs=docs_zip,
    )

    read = await documentation_svc.create_documentation(create)

    return schemas.DocumentationCreateResponse(data=read)
