from zipfile import ZipFile

from app import domain


def get_archive_file_structure(archive: ZipFile) -> list[str]:
    return [item.filename.rstrip("/") for item in archive.filelist if not item.is_dir()]


def map_feature(feature: str) -> domain.Feature:
    parts = feature.split("/")
    return domain.Feature(
        name=parts[-1],
        path=parts[:-1],
    )


def map_features(features: list[str]) -> list[domain.Feature]:
    return [map_feature(f) for f in features]
