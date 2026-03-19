#!/bin/sh
if test -d "$TMPDIR/TestResults/Output"; then
	rm -rf "$TMPDIR/TestResults/Output"
fi

dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile="**/*AppMigrations/*.cs" DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute="GeneratedCodeAttribute,CompilerGeneratedAttribute"

reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"$TMPDIR/TestResults/Output" -reporttypes:Html

if test -d "TestResults"; then
	rm -rf "TestResults"
fi

open "$TMPDIR/TestResults/Output/Index.html"