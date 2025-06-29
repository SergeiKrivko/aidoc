from typing import Optional
from zipfile import ZipFile

from loguru import logger
from openai_proxy import OpenAIProxyClientSettings, OpenAIProxyToolCallClient

from app.core.openai_tool_caller.models import GetFileRequest, GetFileResponse
from app.core.openai_tool_caller.settings import get_openai_tool_caller_settings
from app.core.openai_tool_caller.system_prompts import (
    doc_system_prompts,
    features_system_prompts,
)


class CommonTools:
    def __init__(self, sources: ZipFile, docs: Optional[ZipFile]) -> None:
        self._sources = sources
        self._docs = docs

    async def get_source(self, req: GetFileRequest) -> GetFileResponse:
        try:
            content = self._sources.read(req.path).decode()
        except Exception as e:  # noqa: BLE001
            logger.warning(f"Can not get source: {e}")
            content = None
        return GetFileResponse(content=content)

    async def get_doc(self, req: GetFileRequest) -> GetFileResponse:
        try:
            content = self._docs.read(req.path).decode() if self._docs else None
        except Exception as e:  # noqa: BLE001
            logger.warning(f"Can not get doc: {e}")
            content = None
        return GetFileResponse(content=content)


class FeaturesToolCaller(OpenAIProxyToolCallClient):
    def __init__(self, common_tools: CommonTools) -> None:
        super().__init__(
            system_prompts=features_system_prompts(),
            openai_proxy_client_settings=OpenAIProxyClientSettings(
                base_url=get_openai_tool_caller_settings().base_url,
            ),
        )
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
        super().__init__(
            system_prompts=doc_system_prompts(),
            openai_proxy_client_settings=OpenAIProxyClientSettings(
                base_url=get_openai_tool_caller_settings().base_url,
            ),
        )
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
