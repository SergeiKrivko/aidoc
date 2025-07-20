from typing import Optional
from zipfile import ZipFile

from loguru import logger
from openai_proxy import OpenAIProxyClientSettings

from app.core.openai_tool_caller import generated
from app.core.openai_tool_caller.generated.models import GetFileRequest, GetFileResponse
from app.core.openai_tool_caller.settings import get_openai_tool_caller_settings


class Tools(generated.AbstractTools):
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


class ToolCaller(generated.ToolCaller):
    def __init__(self, sources: ZipFile, docs: Optional[ZipFile]) -> None:
        super().__init__(
            Tools(sources, docs),
            OpenAIProxyClientSettings(
                base_url=get_openai_tool_caller_settings().base_url,
            ),
        )
