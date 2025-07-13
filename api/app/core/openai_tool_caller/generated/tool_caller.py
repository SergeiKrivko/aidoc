import json

from openai_proxy import (
    CodeBlocksParser,
    OpenAIProxyClientSettings,
    OpenAIProxyToolCallClient,
)

from .models import GenerateDocsRequest, GenerateFeaturesRequest
from .prompts import GENERATE_DOCS, GENERATE_FEATURES
from .tools import TOOL_DESCRIPTIONS, AbstractTools


class ToolCaller:
    def __init__(self, tools: AbstractTools, settings: OpenAIProxyClientSettings) -> None:
        OpenAIProxyToolCallClient.mark_tool_methods(tools, TOOL_DESCRIPTIONS)
        client_tools = OpenAIProxyToolCallClient.collect_tools(tools)

        self._generate_features = OpenAIProxyToolCallClient(
            system_prompt_paths=GENERATE_FEATURES,
            openai_proxy_client_settings=settings,
            tools=client_tools,
        )

        self._generate_docs = OpenAIProxyToolCallClient(
            system_prompt_paths=GENERATE_DOCS,
            openai_proxy_client_settings=settings,
            tools=client_tools,
        )

    async def generate_features(self, req: GenerateFeaturesRequest) -> list[str]:
        resp = await self._generate_features.request(req.model_dump_json())
        json_block = CodeBlocksParser(resp).find_json_block()
        return json.loads(json_block)

    async def generate_docs(self, req: GenerateDocsRequest) -> str:
        return await self._generate_docs.request(req.model_dump_json())
