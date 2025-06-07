from dotenv import load_dotenv

try:
    load_dotenv(override=True, dotenv_path="./local.env")
except Exception as e:
    print(e)

OPENAI_TOKEN_VAR = "OPENAI_TOKEN"
DEEPSEEK_TOKEN_VAR = "DEEPSEEK_TOKEN"
DEEPSEEK_URL_VAR = "DEEPSEEK_URL"
LOG_LVL_VAR = "LOG_LVL"
LOG_FOLDER_PATH_VAR = "LOG_FOLDER_PATH"

BFF_DATA_FOLDER_PATH = "./bff_interaction/data"
CONTEXT_FILE_NAME = "context.json"
TOOLS_FILE_NAME = "tools.json"
GPT_RESP_FORMAT_FILE_NAME = "gpt_resp_format.json"