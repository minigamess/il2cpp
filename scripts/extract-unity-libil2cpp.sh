#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${UNITY_VERSION:-}" ]]; then
  echo "UNITY_VERSION is required"
  exit 1
fi

out="/repo/${UNITY_VERSION}"

if [[ -d "${out}" ]]; then
  if [[ "${FORCE:-false}" == "true" ]]; then
    rm -rf "${out}"
  else
    echo "Target folder '${out}' already exists"
    exit 0
  fi
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

modules_src=""
for candidate in \
  "${editor_root}/Data/modules.json" \
  "${editor_root}/modules.json" \
  "/opt/unity/modules.json" \
  "/opt/Unity/modules.json"
do
  if [[ -f "${candidate}" ]]; then
    modules_src="${candidate}"
    break
  fi
done

if [[ -z "${modules_src}" ]]; then
  modules_src="$(find "${editor_root}" /opt/unity /opt/Unity -type f -name modules.json 2>/dev/null | sort | head -n 1)"
fi

if [[ -z "${modules_src}" ]]; then
  echo "Could not locate modules.json"
  exit 1
fi

cp "${modules_src}" "${out}/modules.json"

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

python_bin=""
if command -v python3 >/dev/null 2>&1; then
  python_bin="python3"
elif command -v python >/dev/null 2>&1; then
  python_bin="python"
else
  echo "python is required to parse modules.json"
  exit 1
fi

mac_mono_url="$(${python_bin} - "${out}/modules.json" <<'PY'
import json
import sys

path = sys.argv[1]
with open(path, "r", encoding="utf-8") as f:
    data = json.load(f)

if isinstance(data, list):
    modules = data
elif isinstance(data, dict):
    modules = data.get("modules") or data.get("items") or []
else:
    modules = []

if isinstance(modules, dict):
    modules = list(modules.values())

result = ""
for module in modules:
    if not isinstance(module, dict):
        continue
    if module.get("id") == "mac-mono":
        result = module.get("downloadUrl") or module.get("url") or ""
        if result:
            break

print(result)
PY
)"

if [[ -z "${mac_mono_url}" ]]; then
  echo "Could not find mac-mono download url in modules.json"
  exit 1
fi

changeset="$(printf '%s' "${mac_mono_url}" | sed -E 's#^https?://[^/]+/download_unity/([^/]+)/.*#\1#')"
if [[ -z "${changeset}" || "${changeset}" == "${mac_mono_url}" ]]; then
  echo "Could not parse changeset from mac-mono download url"
  exit 1
fi

mac_il2cpp_url="https://download.unity3d.com/download_unity/${changeset}/MacEditorTargetInstaller/UnitySetup-Mac-IL2CPP-Support-for-Editor-${UNITY_VERSION}.pkg"
linux_il2cpp_url="https://download.unity3d.com/download_unity/${changeset}/LinuxEditorTargetInstaller/UnitySetup-Linux-IL2CPP-Support-for-Editor-${UNITY_VERSION}.tar.xz"

cat > "${out}/download-urls.txt" <<EOF
changeset=${changeset}
mac_mono=${mac_mono_url}
mac_il2cpp=${mac_il2cpp_url}
linux_il2cpp=${linux_il2cpp_url}
EOF
