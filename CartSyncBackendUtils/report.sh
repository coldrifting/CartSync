#!/bin/sh
if test -d "$TMPDIR/TestResults/Output"; then
	rm -rf "$TMPDIR/TestResults/Output"
fi

dotnet test --collect:"XPlat Code Coverage"

reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"$TMPDIR/TestResults/Output" -reporttypes:Html

if test -d "TestResults"; then
	rm -rf "TestResults"
fi

open "$TMPDIR/TestResults/Output/Index.html"