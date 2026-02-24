@ECHO OFF

if exist Migrations rmdir /s /q Migrations

dotnet ef database drop --configuration Debug --force
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet ef migrations add --configuration Debug Initial --output-dir Migrations
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet ef database update
if %errorlevel% neq 0 exit /b %errorlevel%

REM git add Migrations