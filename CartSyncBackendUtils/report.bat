@ECHO OFF

set outputDir="%TEMP%\TestResults\Output"

if exist %outputDir% rmdir /s /q %outputDir%

dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByFile="**/*AppMigrations/*.cs" DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute="GeneratedCodeAttribute,CompilerGeneratedAttribute"

reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"%outputDir%" -reporttypes:Html

if exist "TestResults" rmdir /s /q "TestResults"

explorer "%outputDir%\Index.html"