from abc import ABC, abstractmethod

from .models import GetFileRequest, GetFileResponse


class AbstractTools(ABC):
    @abstractmethod
    async def get_source(self, req: GetFileRequest) -> GetFileResponse:
        pass

    @abstractmethod
    async def get_doc(self, req: GetFileRequest) -> GetFileResponse:
        pass


TOOL_DESCRIPTIONS: dict[str, str] = {
    "get_source": "Считывает содержимое указанного исходного файла.",
    "get_doc": "Считывает содержимое указанного файла документации.",
}
