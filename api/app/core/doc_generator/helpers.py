from zipfile import ZipFile

from app import domain
from app.core.doc_generator.models import Feature


def get_archive_file_structure(archive: ZipFile) -> list[str]:
    return [item.filename.rstrip("/") for item in archive.filelist if not item.is_dir()]


def _map_features_recursively(
    features: list[Feature],
    path: list[str],
) -> list[domain.Feature]:
    mapped = []
    for f in features:
        if f.children:
            mapped.extend(_map_features_recursively(f.children, [*path, f.name]))
        else:
            mapped.append(
                domain.Feature(
                    name=f.name,
                    path=path,
                ),
            )
    return mapped


def map_features(features: list[Feature]) -> list[domain.Feature]:
    return _map_features_recursively(features, [])
