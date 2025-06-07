import os
from pathlib import Path
import fnmatch


def read_gitignore(directory):
    gitignore_path = os.path.join(directory, ".gitignore")
    ignore_patterns = []
    if os.path.isfile(gitignore_path):
        with open(gitignore_path, "r") as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith("#"):
                    ignore_patterns.append(line)
    return ignore_patterns


def is_ignored(path, ignore_patterns, root_dir):
    relative_path = os.path.relpath(path, root_dir)
    for pattern in ignore_patterns:
        # Handle directory patterns (ending with /)
        if pattern.endswith("/"):
            dir_pattern = pattern.rstrip("/")
            if fnmatch.fnmatch(relative_path, dir_pattern) or fnmatch.fnmatch(
                relative_path, f"{dir_pattern}/*"
            ):
                return True
        # Handle regular patterns
        if fnmatch.fnmatch(relative_path, pattern) or fnmatch.fnmatch(
            os.path.basename(path), pattern
        ):
            return True
    return False


def should_include_file(filepath):
    if (
        ".git" in filepath
        or filepath.endswith("combined_code.txt")
        or filepath.endswith("LICENSE")
        or filepath.endswith(".gitignore")
        or filepath.endswith("poetry.lock")
        or filepath.endswith("merge_codebase.py")
    ):
        return False
    return True


def collect_files(root_dir, output_file):
    ignore_patterns = read_gitignore(root_dir)
    with open(output_file, "w", encoding="utf-8") as outfile:
        for root, dirs, files in os.walk(root_dir):
            # Удаляем игнорируемые директории из списка для обхода
            dirs[:] = [
                d
                for d in dirs
                if not is_ignored(os.path.join(root, d), ignore_patterns, root_dir)
            ]

            for file in files:
                filepath = os.path.join(root, file)
                if not is_ignored(
                    filepath, ignore_patterns, root_dir
                ) and should_include_file(filepath):
                    try:
                        with open(filepath, "r", encoding="utf-8") as infile:
                            content = infile.read()
                            relative_path = os.path.relpath(filepath, root_dir)
                            outfile.write(f"=== {relative_path} ===\n")
                            outfile.write(content)
                            outfile.write("\n\n")
                    except (UnicodeDecodeError, PermissionError):
                        # Пропускаем бинарные файлы и файлы без доступа
                        continue


if __name__ == "__main__":
    project_root = os.getcwd()  # Можно заменить на конкретный путь
    output_filename = "../combined_code.txt"
    collect_files(project_root, output_filename)
    print(f"Все файлы проекта объединены в {output_filename}")
