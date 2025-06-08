from app.clients.openai_client import MessageModel, OpenAIRole, MessagesModel


def read_from_file(file_path: str) -> str:
    with open(file=file_path, mode="r", encoding="utf-8") as file:
        return file.read().replace("\n", "")


# контексты работы агента
AGENT_ROLE_CONTEXT = MessageModel(
    role=OpenAIRole.SYSTEM,
    content=read_from_file("bff_interaction/data/role.txt"),
)

RESP_RULES_CONTEXT = MessageModel(
    role=OpenAIRole.SYSTEM,
    content=read_from_file("bff_interaction/data/response_rules.txt"),
)


INIT_CONTEXT = MessagesModel(root=[AGENT_ROLE_CONTEXT, RESP_RULES_CONTEXT])


# контексты работы агента
UML_ROLE_CONTEXT = MessageModel(
    role=OpenAIRole.SYSTEM,
    content=read_from_file("bff_interaction/data/uml_role.txt"),
)

UML_RULES_CONTEXT = MessageModel(
    role=OpenAIRole.SYSTEM,
    content=read_from_file("bff_interaction/data/uml_rules.txt"),
)


UML_CONTEXT = MessagesModel(root=[UML_ROLE_CONTEXT, UML_RULES_CONTEXT])
