#!/usr/bin/env bash
dotnet ef database drop
rm -rf migrations
dotnet ef migrations add Initial
dotnet ef database update
