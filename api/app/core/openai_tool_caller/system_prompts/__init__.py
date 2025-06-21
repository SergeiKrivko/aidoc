# ruff: noqa
# todo временный код, по-хорошему стоит добавить в openai-proxy поддержку файлов

from functools import lru_cache
from pathlib import Path


@lru_cache
def features_system_prompts() -> list[str]:
    system_prompt_paths = [
        Path(__file__).parent / "features_role.txt",
        Path(__file__).parent / "features_rules.txt",
    ]
    prompts = []
    for path in system_prompt_paths:
        with open(path) as file:
            prompts.append(file.read().strip())
    return prompts


@lru_cache
def doc_system_prompts() -> list[str]:
    system_prompt_paths = [
        Path(__file__).parent / "doc_role.txt",
        Path(__file__).parent / "doc_rules.txt",
    ]
    prompts = []
    for path in system_prompt_paths:
        with open(path) as file:
            prompts.append(file.read().strip())
    return prompts
