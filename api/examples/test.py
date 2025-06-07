import json


with open(file="resp.json", mode="r", encoding="utf-8") as file:
    data = json.load(file)

for key, value in data.items():
    with open(f"{key}", "w", encoding='utf-8') as file:
        file.write(value)

