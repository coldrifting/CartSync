@ECHO OFF

set MigrationFolder="AppMigrations"

if exist "%MigrationFolder%" rmdir /s /q "%MigrationFolder%"

dotnet ef database drop --force
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet ef migrations add Initial --output-dir "%MigrationFolder%"
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet ef database update
if %errorlevel% neq 0 exit /b %errorlevel%

git add "%MigrationFolder%"