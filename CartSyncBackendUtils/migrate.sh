#!/bin/sh
migrationFolder="AppMigrations"

if test -d "$migrationFolder"; then
	rm -rf "$migrationFolder"
fi

if ! dotnet ef database drop --force; then
	exit 1
fi

if ! dotnet ef migrations add Initial --output-dir "$migrationFolder"; then
	exit 1
fi

if ! dotnet ef database update; then
	exit 1
fi

git add "$migrationFolder"