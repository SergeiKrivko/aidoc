# ruff: noqa
from functools import lru_cache

from app.settings.env_settings import EnvSettings


class MetricsSettings(EnvSettings):
    excluded_handlers: list[str] = [
        "/healthz",
        "/docs",
        "/metrics",
        "/readyz",
        "/openapi.json",
    ]
    latency_lowr_buckets: list[float] = [
        0.1,
        0.25,
        0.5,
        0.75,
        1.0,
        1.5,
        2.0,
        2.5,
        5.0,
        7.5,
        10.0,
        20.0,
        30.0,
    ]


@lru_cache
def get_metrics_settings() -> MetricsSettings:
    return MetricsSettings()
