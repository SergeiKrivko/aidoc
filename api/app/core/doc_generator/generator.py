from functools import lru_cache
from zipfile import ZipFile

from app.core.doc_generator.helpers import get_archive_file_structure, map_features
from app.core.doc_generator.models import GenerateDoc
from app.core.openai_tool_caller import ToolCaller, models


class DocGenerator:
    async def generate(
        self,
        dst: ZipFile,
        data: GenerateDoc,
    ) -> ZipFile:
        tool_caller = ToolCaller(data.sources, data.docs)

        structure_sources = get_archive_file_structure(data.sources)
        structure_docs = get_archive_file_structure(data.docs) if data.docs else []

        features = await tool_caller.generate_features(
            models.GenerateFeaturesRequest(
                name=data.info.application_info.name,
                structure_sources=structure_sources,
                structure_docs=structure_docs,
                changed_sources=data.info.changed_sources,
                changed_docs=data.info.changed_docs,
            ),
        )

        for feature in map_features(features):
            current_doc = data.docs.read(feature.doc_path).decode() if data.docs else None
            new_doc = await tool_caller.generate_docs(
                models.GenerateDocsRequest(
                    name=data.info.application_info.name,
                    structure_sources=structure_sources,
                    changed_sources=data.info.changed_sources,
                    feature=feature.name,
                    current_doc=current_doc,
                ),
            )
            dst.writestr(f"docs/{feature.doc_path}", new_doc)

        return dst


@lru_cache
def get_doc_generator() -> DocGenerator:
    return DocGenerator()
