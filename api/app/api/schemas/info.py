from pydantic import BaseModel, HttpUrl


class Info(BaseModel):
    name: str
    github_repo: str

    def github_repo_name(self) -> str:
        """
        Получаем название репы из ссылки.
        По-хорошему, надо сделать так, чтобы github_repo изначально был HttpUrl.
        Но это позже.
        :return:
        """
        try:
            repo_url = HttpUrl(self.github_repo)
            path_segments = [s for s in repo_url.path.split("/") if s]
            if not path_segments:
                raise ValueError
        except ValueError:
            print(f"invalid github_repo: {self.github_repo}")
            return self.name
        else:
            return path_segments[-1]
