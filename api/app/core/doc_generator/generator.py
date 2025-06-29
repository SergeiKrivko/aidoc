from functools import lru_cache
from zipfile import ZipFile

from loguru import logger
from openai_proxy import CodeBlocksParser

from app.core.doc_generator.helpers import get_archive_file_structure, map_features
from app.core.doc_generator.models import (
    DocRequest,
    Feature,
    FeaturesRequest,
    GenerateDoc,
)
from app.core.openai_tool_caller import (
    CommonTools,
    DocToolCaller,
    FeaturesToolCaller,
    GetFileRequest,
)


class DocGenerator:
    async def generate(
        self,
        dst: ZipFile,
        data: GenerateDoc,
    ) -> ZipFile:
        common_tools = CommonTools(data.sources, data.docs)

        structure_sources = get_archive_file_structure(data.sources)
        structure_docs = get_archive_file_structure(data.docs) if data.docs else []

        features = await self._generate_features(
            req=FeaturesRequest(
                name=data.info.application_info.name,
                structure_sources=structure_sources,
                structure_docs=structure_docs,
                changed_sources=data.info.changed_sources,
                changed_docs=data.info.changed_docs,
            ),
            common_tools=common_tools,
        )

        for feature in map_features(features):
            current_doc = await common_tools.get_doc(
                GetFileRequest(path=feature.doc_path),
            )
            new_doc = await self._generate_doc(
                req=DocRequest(
                    name=data.info.application_info.name,
                    structure_sources=structure_sources,
                    changed_sources=data.info.changed_sources,
                    feature=feature.name,
                    current_doc=current_doc.content,
                ),
                common_tools=common_tools,
            )
            dst.writestr(f"docs/{feature.doc_path}", new_doc)

        return dst

    async def _generate_features(
        self,
        req: FeaturesRequest,
        common_tools: CommonTools,
    ) -> list[Feature]:
        tool_caller = FeaturesToolCaller(common_tools)
        resp = await tool_caller.request(req.model_dump_json())

        json_blocks = CodeBlocksParser(resp).find_json_blocks()
        if len(json_blocks) > 1:
            warn = "More than one json block for features found, using only first one"
            logger.warning(warn)

        return [Feature.model_validate(f) for f in json_blocks[0]]

    async def _generate_doc(
        self,
        req: DocRequest,
        common_tools: CommonTools,
    ) -> str:
        tool_caller = DocToolCaller(common_tools)
        doc = await tool_caller.request(req.model_dump_json())
        logger.info(f"Doc for feature {req.feature} generated successfully")
        return doc


@lru_cache
def get_doc_generator() -> DocGenerator:
    return DocGenerator()
