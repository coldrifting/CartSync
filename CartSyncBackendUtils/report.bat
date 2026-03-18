@ECHO OFF

set outputDir="%TEMP%\TestResults\Output"

if exist %outputDir% rmdir /s /q %outputDir%

dotnet test --collect:"XPlat Code Coverage"

reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"%outputDir%" -reporttypes:Html

if exist "TestResults" rmdir /s /q "TestResults"

explorer "%outputDir%\Index.html"