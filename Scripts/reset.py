#!/usr/bin/python3

from pathlib import Path
from inspect import currentframe, getframeinfo
import subprocess

scriptFile = getframeinfo(currentframe()).filename
rootDir = Path(scriptFile).resolve().parent.parent

apiPath = rootDir / "API" / "CartSync"
projectPath = apiPath / "CartSync.csproj"

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
                "database",
                "update",
                "--project",
                str(projectPath)],
               check=True)