import io
from functools import lru_cache
from zipfile import ZIP_DEFLATED, ZipFile

from app import domain


class DocArchiver:
    async def archive(self, docs: list[domain.DocGeneratedFile]) -> bytes:
        buffer = io.BytesIO()

        with ZipFile(buffer, mode="w", compression=ZIP_DEFLATED) as zip_file:
            for doc in docs:
                zip_file.writestr(doc.path, doc.content)

        buffer.seek(0)
        return buffer.getvalue()


@lru_cache
def get_doc_archiver() -> DocArchiver:
    return DocArchiver()
