#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

SOURCE_ROOT="$REPO_ROOT/Assets/ParticleThumbnail&Preview"
DOCS_ROOT="$SOURCE_ROOT/Documentation"
PACKAGE_ROOT="$REPO_ROOT/upm/com.fardinhaque.particle-thumbnail-preview"
TOOL_FOLDER_NAME="ParticleThumbnail&Preview"
TOOL_PACKAGE_ROOT="$PACKAGE_ROOT/$TOOL_FOLDER_NAME"

if [[ ! -d "$SOURCE_ROOT" ]]; then
  echo "Source folder not found: $SOURCE_ROOT" >&2
  exit 1
fi

mkdir -p "$PACKAGE_ROOT"

echo "Syncing UPM package editor code from: $SOURCE_ROOT"

rm -rf "$PACKAGE_ROOT/Editor" "$TOOL_PACKAGE_ROOT"

rsync -a --delete \
  --exclude '.DS_Store' \
  --exclude 'Documentation/' \
  --exclude 'Documentation.meta' \
  --exclude 'Editor/Common/Tests/' \
  --exclude 'Editor/Common/Tests.meta' \
  "$SOURCE_ROOT/" \
  "$TOOL_PACKAGE_ROOT/"

rm -rf "$TOOL_PACKAGE_ROOT/Editor/Common/Tests" "$TOOL_PACKAGE_ROOT/Editor/Common/Tests.meta"

if [[ -d "$DOCS_ROOT" ]]; then
  for doc in README.md CHANGELOG.md THIRD_PARTY_NOTICES.md; do
    if [[ -f "$DOCS_ROOT/$doc" ]]; then
      cp "$DOCS_ROOT/$doc" "$PACKAGE_ROOT/$doc"
    fi
  done
else
  echo "Warning: documentation folder not found at $DOCS_ROOT" >&2
fi

echo "UPM sync complete: $PACKAGE_ROOT"
