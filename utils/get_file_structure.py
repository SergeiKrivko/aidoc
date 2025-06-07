import json
import os


def get_project_structure(root_dir, ignore_dirs=None, ignore_files=None):
    """
    Генерирует структуру проекта в виде относительных путей

    :param root_dir: Корневая директория проекта
    :param ignore_dirs: Список директорий для игнорирования (например, ['.git', '__pycache__'])
    :param ignore_files: Список файлов для игнорирования (например, ['.gitignore', '.DS_Store'])
    :return: Генератор относительных путей файлов и директорий
    """
    if ignore_dirs is None:
        ignore_dirs = []
    if ignore_files is None:
        ignore_files = []

    for dirpath, dirnames, filenames in os.walk(root_dir, topdown=True):
        # Удаляем игнорируемые директории из списка
        dirnames[:] = [d for d in dirnames if d not in ignore_dirs]

        # Получаем относительный путь
        rel_path = os.path.relpath(dirpath, root_dir)
        if rel_path == ".":
            rel_path = ""

        # Выводим текущую директорию
        if rel_path:
            yield rel_path + os.sep

        # Выводим файлы в текущей директории
        for filename in filenames:
            if filename not in ignore_files and all(
                d not in filename for d in ignore_dirs
            ):
                if rel_path:
                    yield os.path.join(rel_path, filename)
                else:
                    yield filename


def save_structure_to_file(
    root_dir, output_file="project_structure.txt", ignore_dirs=None, ignore_files=None
):
    """
    Сохраняет структуру проекта в файл

    :param root_dir: Корневая директория проекта
    :param output_file: Имя выходного файла
    :param ignore_dirs: Список директорий для игнорирования
    :param ignore_files: Список файлов для игнорирования
    """
    with open(output_file, "w", encoding="utf-8") as f:
        items = list(
            filter(
                lambda n: not n.endswith("/"),
                get_project_structure(root_dir, ignore_dirs, ignore_files),
            )
        )
        json.dump(items, f)
    print(f"Структура проекта сохранена в файл: {output_file}")


if __name__ == "__main__":
    # Пример использования
    project_dir = ".."  # Текущая директория
    output_filename = "../project_structure.txt"

    # Директории и файлы для игнорирования
    ignore_directories = [".git", "__pycache__", ".idea", ".venv"]
    ignore_file_list = [".gitignore", ".DS_Store", "*.pyc"]

    save_structure_to_file(
        project_dir,
        output_filename,
        ignore_dirs=ignore_directories,
        ignore_files=ignore_file_list,
    )
