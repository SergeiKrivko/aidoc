from pathlib import Path

GENERATE_FEATURES = [
    Path(__file__).parent.parent / "system_prompts" / "features_role.txt",
    Path(__file__).parent.parent / "system_prompts" / "features_rules.txt",
]


GENERATE_DOCS = [
    Path(__file__).parent.parent / "system_prompts" / "doc_role.txt",
    Path(__file__).parent.parent / "system_prompts" / "doc_rules.txt",
]
