from loguru import logger
from pydantic import BaseModel, HttpUrl


class AppInfo(BaseModel):
    name: str
    github_repo: HttpUrl = HttpUrl("https://github.com/SergeiKrivko/aidoc")

    def github_repo_name(self) -> str:
        try:
            path = self.github_repo.path or ""
            path_segments = [s for s in path.split("/") if s]
            return path_segments[-1]
        except ValueError as e:
            logger.error(f"Invalid github repo url: {e}")
            return self.name
