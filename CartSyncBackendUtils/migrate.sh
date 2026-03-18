#!/bin/sh
if test -d "Migrations"; then
	rm -rf "Migrations"
fi

if ! dotnet ef database drop --force; then
	exit 1
fi

if ! dotnet ef migrations add Initial --output-dir Migrations; then
	exit 1
fi

if ! dotnet ef database update; then
	exit 1
fi

git add "Migrations"