from pydantic import BaseModel


class Info(BaseModel):
    name: str
    github_repo: str
