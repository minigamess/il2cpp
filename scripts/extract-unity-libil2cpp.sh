#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${UNITY_VERSION:-}" ]]; then
  echo "UNITY_VERSION is required"
  exit 1
fi

out="/repo/${UNITY_VERSION}"

if [[ -d "${out}" ]]; then
  echo "Target folder '${out}' already exists"
  exit 0
fi

mkdir -p "${out}"

editor_root=""
for candidate in /opt/unity/Editor /opt/Unity/Editor; do
  if [[ -d "${candidate}/Data/il2cpp/libil2cpp" ]]; then
    editor_root="${candidate}"
    break
  fi
done

if [[ -z "${editor_root}" ]]; then
  echo "Could not locate libil2cpp"
  exit 1
fi

mkdir -p "${out}/Editor/Data/il2cpp"
cp -a "${editor_root}/Data/il2cpp/libil2cpp" "${out}/Editor/Data/il2cpp/"

baselib_path="$(find "${editor_root}/Data/PlaybackEngines/LinuxStandaloneSupport/Variations" -type f -name baselib.a | sort | head -n 1)"
if [[ -z "${baselib_path}" ]]; then
  echo "Could not find baselib.a"
  exit 1
fi

baselib_rel="${baselib_path#${editor_root}/Data/}"
baselib_out="${out}/Editor/Data/${baselib_rel}"
mkdir -p "$(dirname "${baselib_out}")"
cp "${baselib_path}" "${baselib_out}"
