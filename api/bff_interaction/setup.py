from datetime import datetime, timezone, timedelta
from typing import Any
import json

from app.clients.openai_client import MessageModel, OpenAIRole, MessagesModel


def get_current_date() -> str:
    return datetime.now().astimezone(timezone(timedelta(hours=3))).isoformat()


def read_from_file(file_path: str) -> str:
    with open(file=file_path, mode="r", encoding="utf-8") as file:
        return file.read().replace("\n", "")


def read_json_from_file(file_path: str) -> dict[str, Any]:
    with open(file=file_path, mode="r", encoding="utf-8") as file:
        return json.load(fp=file)


# контексты работы агента
AGENT_ROLE_CONTEXT = MessageModel(
    role=OpenAIRole.SYSTEM, content=read_from_file("bff_interaction/data/role.txt")
)

RESP_RULES_CONTEXT = MessageModel(
    role=OpenAIRole.SYSTEM,
    content=read_from_file("bff_interaction/data/response_rules.txt"),
)


INIT_CONTEXT = MessagesModel(root=[AGENT_ROLE_CONTEXT, RESP_RULES_CONTEXT])
