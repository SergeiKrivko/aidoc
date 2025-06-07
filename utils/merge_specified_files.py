files = [
    "api/app/domain/doc_nodes.py",
    "api/app/api/app.py",
    "api/app/settings.py",
    "api/app/protocols/doc_storage.py",
    "api/app/protocols/source_loader.py",
]
contents = []

for file in files:
    with open(file, "r", encoding="utf-8") as f:
        content = f.read()
        contents.append(file + "\n")
        contents.append(content + "\n")
        contents.append("\n")

with open("../merged_file.txt", "w", encoding="utf-8") as f:
    f.write("".join(contents))
