from datetime import datetime, timezone, timedelta
from typing import Any
import json

from bff_interaction.schema import ResponseFormat
from openai_api.schema import *
from utils.logger import logger


def get_current_date() -> str:
    return datetime.now().astimezone(timezone(timedelta(hours=3))).isoformat()


def read_from_file(file_path: str) -> str:
    with open(file=file_path, mode='r', encoding='utf-8') as file:
        return file.read().replace('\n', '')
    

def read_json_from_file(file_path: str) -> dict[str, Any]:
    with open(file=file_path, mode='r', encoding='utf-8') as file:
        return json.load(fp=file)


# RESP_SCHEMA = str(ResponseFormat.model_json_schema())
# SCHEDULE_SCHEMA = str(read_json_from_file("bff_interaction/data/schedule_schema.json"))

# logger.logger.info(f"RESP_SCHEMA: {RESP_SCHEMA}")
# logger.logger.info(f"SCHEDULE_SCHEMA: {SCHEDULE_SCHEMA}")

# контексты работы агента
AGENT_ROLE_CONTEXT = MessageModel(role=OpenAIRole.SYSTEM,
                                   content=read_from_file("bff_interaction/data/role.txt"))

# RESP_FORMAT_CONTEXT = MessageModel(role=OpenAIRole.SYSTEM,
#                                    content=f"В ответ всегда возвращай сообщение в json формате по следующей схеме:{RESP_SCHEMA}"
# )

# RESP_DETAILS_CONTEXT = MessageModel(role=OpenAIRole.SYSTEM,
#                                     content=read_from_file("bff_interaction/data/response_details.txt"))

# SCHEDULE_SCHEMA_CONTEXT = MessageModel(role=OpenAIRole.SYSTEM,
#                                        content=f"поле shedule должно соответствовать следующей схеме: {SCHEDULE_SCHEMA}")

RESP_RULES_CONTEXT = MessageModel(role=OpenAIRole.SYSTEM,
                                    content=read_from_file("bff_interaction/data/response_rules.txt"))


INIT_CONTEXT = MessagesModel(
    root=[AGENT_ROLE_CONTEXT, RESP_RULES_CONTEXT]
)


