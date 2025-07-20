import json
import os
import sys
from typing import Any

from loguru import logger as logger_clt
from loguru._logger import Logger as LoggerInst

from app.settings import log_settings


class Logger:
    __instance: "Logger" = None

    def __new__(cls) -> "Logger":
        if cls.__instance is None:
            cls.__instance = super().__new__(cls)
            cls.__instance.setup_logger()

        return cls.__instance

    def setup_logger(self) -> None:
        self.log_folder_path: str = log_settings.get_log_settings().folder_path
        os.makedirs(
            self.log_folder_path,
            exist_ok=True,
        )  # создаем папку если такой не существует
        self.log_file_path: str = os.path.join(self.log_folder_path, "app_{time}.log")
        self.log_lvl: str = log_settings.get_log_settings().level

        logger_clt.remove()

        logger_clt.add(sys.stdout, level=self.log_lvl)
        logger_clt.add(
            self.log_file_path,
            rotation="100 KB",
            retention="14 days",
            level=self.log_lvl,
            encoding="utf-8",
            enqueue=True,
        )

        self.logger = logger_clt

    def log_json(self, data: dict[str, Any]) -> None:
        with open(
            os.path.join(self.log_folder_path, "data.json"),
            "w",
            encoding="utf-8",
        ) as file:
            json.dump(fp=file, obj=data, indent=4, ensure_ascii=False)

    def get_logger(self) -> LoggerInst:
        return self.logger


logger = Logger()
