from loguru import logger
from openai_proxy import OpenAIProxyToolCallClient

from app.api import schemas
from app.core.openai_tool_caller.models import GetFileRequest, GetFileResponse
from app.core.openai_tool_caller.system_prompts import (
    doc_system_prompts,
    features_system_prompts,
)


class CommonTools:
    def __init__(self, data: schemas.DocCreate):
        self._data = data

    async def get_source(self, req: GetFileRequest) -> GetFileResponse:
        try:
            content = self._data.sources.read(req.path).decode()
        except Exception as e:  # noqa: BLE001
            logger.warning(f"Can not get source: {e}")
            content = None
        return GetFileResponse(content=content)

    async def get_doc(self, req: GetFileRequest) -> GetFileResponse:
        try:
            content = (
                self._data.docs.read(req.path).decode() if self._data.docs else None
            )
        except Exception as e:  # noqa: BLE001
            logger.warning(f"Can not get doc: {e}")
            content = None
        return GetFileResponse(content=content)


class FeaturesToolCaller(OpenAIProxyToolCallClient):
    def __init__(self, common_tools: CommonTools) -> None:
        super().__init__(features_system_prompts())
        self._common_tools = common_tools

    @OpenAIProxyToolCallClient.tool(
        description="Считывает содержимое указанного исходного файла.",
    )
    async def get_source(self, req: GetFileRequest) -> GetFileResponse:
        return await self._common_tools.get_source(req)

    @OpenAIProxyToolCallClient.tool(
        description="Считывает содержимое указанного файла документации.",
    )
    async def get_doc(self, req: GetFileRequest) -> GetFileResponse:
        return await self._common_tools.get_doc(req)


class DocToolCaller(OpenAIProxyToolCallClient):
    def __init__(self, common_tools: CommonTools) -> None:
        super().__init__(doc_system_prompts())
        self._common_tools = common_tools

    @OpenAIProxyToolCallClient.tool(
        description="Считывает содержимое указанного исходного файла.",
    )
    async def get_source(self, req: GetFileRequest) -> GetFileResponse:
        return await self._common_tools.get_source(req)

    @OpenAIProxyToolCallClient.tool(
        description="Считывает содержимое указанного файла документации.",
    )
    async def get_doc(self, req: GetFileRequest) -> GetFileResponse:
        return await self._common_tools.get_doc(req)
