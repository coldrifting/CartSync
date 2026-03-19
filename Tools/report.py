#!/usr/bin/python3

from pathlib import Path
from inspect import currentframe, getframeinfo
import tempfile
import webbrowser
import shutil
import subprocess


def get_first_subdir(path):
    p = Path(path)
    try:
        # Use next() with a generator to find the first directory
        first_subdir = next(entry for entry in p.iterdir() if entry.is_dir())
        return first_subdir
    except StopIteration:
        return None # No subdirectories found
    except FileNotFoundError:
        return None # The initial path does not exist
    
scriptFile = getframeinfo(currentframe()).filename
rootDir = Path(scriptFile).resolve().parent.parent / "API"

runSettings = rootDir / "CartSync.runsettings"
testsPath = rootDir / "CartSyncTests"
testResultsPath = testsPath / "TestResults"

outputDir = Path(tempfile.gettempdir()).resolve() / "TestResults" / "Output"
reportFile = outputDir / "Index.html"

if outputDir.is_dir():
    shutil.rmtree(outputDir)
    print(f"Directory '{outputDir}' Deleted")

subprocess.run(["dotnet",
                         "test",
                         str(rootDir),
                         "--settings",
                         str(runSettings)],
                        check=True)

coverageFile = str(get_first_subdir(testResultsPath) / "coverage.cobertura.xml")

subprocess.run(["reportgenerator",
                f"-reports:\"{coverageFile}\"",
                f"-targetdir:{outputDir}",
                "-reporttypes:Html"],
               check=True)

if testResultsPath.is_dir():
    shutil.rmtree(testResultsPath)
    print(f"Directory '{testResultsPath}' Deleted")

webbrowser.open(str(reportFile), new=0, autoraise=True)