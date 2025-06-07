from typing import Any
import json
import os
from copy import deepcopy

from openai_api.schema import *
from bff_interaction.setup import INIT_CONTEXT
from config import *


class Client:
    def __init__(self,
                 data_folder_path: str = BFF_DATA_FOLDER_PATH,
                 context_file_name: str = CONTEXT_FILE_NAME,
                 tools_file_name: str = TOOLS_FILE_NAME,
                 gpt_format_file_name: str = GPT_RESP_FORMAT_FILE_NAME):
        
        self.data_folder_path = data_folder_path
        self.context_file_path = os.path.join(data_folder_path, context_file_name)
        self.tools_file_path = os.path.join(data_folder_path, tools_file_name)
        self.gpt_format_file_path = os.path.join(data_folder_path, gpt_format_file_name)
    
    def read_json(self, file_path: str) -> dict[str, Any]:
        with open(file=file_path, mode='r', encoding='utf-8') as file:
            return json.load(fp=file)
    
    def _get_resp_format(self, file_path: str = None) -> dict[str, Any]:
        raise DeprecationWarning()
        if file_path is None:
            file_path = self.gpt_format_file_path
        return self.read_json(file_path)

    def get_context(self) -> MessagesModel:
        context = deepcopy(INIT_CONTEXT)
        # context._update_date()
        
        return context

    def get_tools(self, file_path: str = None) -> ToolsModel:
        if file_path is None:
            file_path = self.tools_file_path
        
        json_data = self.read_json(file_path)
        return ToolsModel.model_validate(json_data)
        
    