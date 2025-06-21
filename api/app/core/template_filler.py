import pathlib
import shutil
import uuid
from functools import lru_cache

import jinja2

from app.api import schemas


class TemplateFiller:
    root_path = pathlib.Path(__file__).parent.parent.parent
    static_dir_path = root_path / "static"
    config_relative_path = "docusaurus.config.ts"

    def fill(self, info: schemas.Info) -> bytes:
        """
        Архив байтами
        :param info:
        :return:
        """
        dirname = str(uuid.uuid4())
        new_path = self.root_path / dirname
        shutil.copytree(self.static_dir_path, new_path)

        config_path = new_path / self.config_relative_path
        config_path.write_text(self.fill_config(info))

        archive_name = shutil.make_archive(dirname, "zip", new_path)
        with open(archive_name, "rb") as f:
            return f.read()

    def fill_config(self, info: schemas.Info) -> str:
        config_template_path = self.static_dir_path / self.config_relative_path
        template = jinja2.Template(config_template_path.read_text())
        return template.render(info=info)


@lru_cache
def get_template_filler() -> TemplateFiller:
    return TemplateFiller()
