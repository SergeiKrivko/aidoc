import aiohttp
from pathlib import Path
from pydantic import HttpUrl
from app.api import schemas


async def main():
    doc_info = schemas.DocInfo(
        application_info=schemas.AppInfo(
            name="test-app",
            github_repo=HttpUrl("https://github.com/SergeiKrivko/aidoc"),
        ),
        changed_sources=["test.py"],
        changed_docs=[],
    )

    sources_filename = "sources.zip"
    sources_path = Path(__file__).parent / sources_filename
    sources = sources_path.read_bytes()

    with aiohttp.MultipartWriter("form-data") as mp:
        info_part = mp.append(
            doc_info.model_dump_json(), {"content-type": "application/json"}
        )
        info_part.set_content_disposition("form-data", name="info")

        sources_part = mp.append(sources, {"content-type": "application/octet-stream"})
        sources_part.set_content_disposition(
            "form-data",
            name="sources",
            filename=sources_filename,
        )

    async with aiohttp.ClientSession(
        "https://aidoc-api.nachert.art",
    ) as session, session.post("/api/v1/documentation", data=mp) as resp:
        doc_task = await resp.json()
        print(doc_task)


if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
