from functools import lru_cache
from typing import Any
import json
import os
from copy import deepcopy

from app.clients.openai_client import MessagesModel, ToolsModel
from app.settings import BFFSettings
from app.settings.bff_settings import get_bff_settings
from bff_interaction.setup import INIT_CONTEXT


class Client:
    def __init__(self, settings: BFFSettings) -> None:
        self.data_folder_path = settings.data_folder_path
        self.context_file_path = os.path.join(
            self.data_folder_path, settings.context_file_name
        )
        self.tools_file_path = os.path.join(
            self.data_folder_path, settings.tools_file_name
        )
        self.gpt_format_file_path = os.path.join(
            self.data_folder_path, settings.gpt_format_file_name
        )

    def read_json(self, file_path: str) -> dict[str, Any]:
        with open(file=file_path, mode="r", encoding="utf-8") as file:
            return json.load(fp=file)

    def get_context(self) -> MessagesModel:
        context = deepcopy(INIT_CONTEXT)
        return context

    def get_tools(self, file_path: str = None) -> ToolsModel:
        if file_path is None:
            file_path = self.tools_file_path

        json_data = self.read_json(file_path)
        return ToolsModel.model_validate(json_data)


@lru_cache
def get_bff_client() -> Client:
    return Client(get_bff_settings())
