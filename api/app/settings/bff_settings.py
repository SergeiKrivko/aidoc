from pydantic_settings import SettingsConfigDict

from app.settings.env_settings import EnvSettings


class BFFSettings(EnvSettings):
    model_config = SettingsConfigDict(
        env_prefix="BFF__",
    )

    data_folder_path: str = "./bff_interaction/data"
    context_file_name: str = "context.json"
    tools_file_name: str = "tools.json"
    gpt_format_file_name: str = "gpt_resp_format.json"


def get_bff_settings() -> BFFSettings:
    return BFFSettings()
