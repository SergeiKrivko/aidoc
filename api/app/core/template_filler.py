import pathlib
from functools import lru_cache

import jinja2
from app.api import schemas


class TemplateFiller:
    static_dir_path = pathlib.Path(__file__).parent.parent.parent / "static"
    config_path = static_dir_path / "docusaurus.config.ts"

    def fill(self, info: schemas.Info):
        print(info.model_dump())

    def fill_config(self, info: schemas.Info) -> str:
        template = jinja2.Template(self.config_path.read_text())
        return template.render(info=info)


@lru_cache
def get_template_filler() -> TemplateFiller:
    return TemplateFiller()
