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

data_root=""
for candidate in /opt/unity/Editor/Data /opt/Unity/Editor/Data; do
  if [[ -d "${candidate}/il2cpp/libil2cpp" ]]; then
    data_root="${candidate}"
    break
  fi
done

if [[ -z "${data_root}" ]]; then
  echo "Could not locate libil2cpp"
  exit 1
fi

cp -a "${data_root}/il2cpp/libil2cpp" "${out}/"

baselib_path="$(find "${data_root}/PlaybackEngines/LinuxStandaloneSupport/Variations" -type f -name baselib.a | sort | head -n 1)"
if [[ -z "${baselib_path}" ]]; then
  echo "Could not find baselib.a"
  exit 1
fi

cp "${baselib_path}" "${out}/baselib.a"
