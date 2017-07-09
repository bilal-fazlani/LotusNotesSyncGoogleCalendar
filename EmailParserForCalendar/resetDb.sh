#!/usr/bin/env bash -xe
dotnet ef database drop
rm -rf migrations
dotnet ef migrations add Initial
dotnet ef database update
