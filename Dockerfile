#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . ./Core.InterviewPrep.PostgreSQL/
WORKDIR /source/Core.InterviewPrep.PostgreSQL
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# runs it using aspnet runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Core.InterviewPrep.PostgreSQL.dll"]
