#!/usr/bin/python3

from pathlib import Path
from inspect import currentframe, getframeinfo
import shutil
import subprocess

migrationFolder = "_Migrations"

scriptFile = getframeinfo(currentframe()).filename
rootDir = Path(scriptFile).resolve().parent.parent

apiPath = rootDir / "API" / "CartSync"
projectPath = apiPath / "CartSync.csproj"
migrationPath = apiPath / migrationFolder

if migrationPath.is_dir():
    shutil.rmtree(migrationPath)
    print(f"Directory '{migrationPath}' Deleted")

subprocess.run(["dotnet",
                "ef",
                "database",
                "drop",
                "--force",
                "--project",
                str(projectPath)],
               check=True)

subprocess.run(["dotnet",
                "ef",
                "migrations",
                "add",
                "Initial",
                "--project",
                str(projectPath),
                "--no-build",
                "--output-dir",
                str(migrationPath)],
               check=True)

subprocess.run(["dotnet",
                "ef",
                "database",
                "update",
                "--project",
                str(projectPath)],
               check=True)

subprocess.run(["git", "add", str(migrationPath)])
