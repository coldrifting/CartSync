#!/bin/sh
if test -d "Migrations"; then
	rm -rf "Migrations"
fi

if ! dotnet ef database drop --configuration Debug --force; then
	exit 1
fi

if ! dotnet ef migrations add --configuration Debug Initial --output-dir Migrations; then
	exit 1
fi

git add "Migrations"