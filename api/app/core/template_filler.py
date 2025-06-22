from functools import lru_cache
from pathlib import Path
from zipfile import ZipFile

import jinja2

from app.api import schemas


class TemplateFiller:
    root_path = Path(__file__).parent.parent.parent
    static_dir_path = root_path / "static"
    config_relative_path = "docusaurus.config.ts"

    async def fill(self, dst: ZipFile, info: schemas.AppInfo) -> ZipFile:
        # Копируем всю статику в архив
        for file in self.static_dir_path.rglob("*"):
            if file.is_file() and file.name != self.config_relative_path:
                relative_path = file.relative_to(self.static_dir_path)
                dst.write(file, arcname=relative_path)

        # Заполняем конфиг и закидываем в архив
        config_data = self.fill_config(info).encode("utf-8")
        dst.writestr(self.config_relative_path, config_data)
        return dst

    def fill_config(self, info: schemas.AppInfo) -> str:
        config_template_path = self.static_dir_path / self.config_relative_path
        template = jinja2.Template(config_template_path.read_text())
        return template.render(info=info)


@lru_cache
def get_template_filler() -> TemplateFiller:
    return TemplateFiller()
