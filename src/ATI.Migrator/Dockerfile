#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ATI.Migrator/ATI.Migrator.csproj", "src/ATI.Migrator/"]
COPY ["src/ATI.EntityFrameworkCore/ATI.EntityFrameworkCore.csproj", "src/ATI.EntityFrameworkCore/"]
COPY ["src/ATI.Core/ATI.Core.csproj", "src/ATI.Core/"]
COPY ["src/ATI.Core.Shared/ATI.Core.Shared.csproj", "src/ATI.Core.Shared/"]
RUN dotnet restore "src/ATI.Migrator/ATI.Migrator.csproj"
COPY . .
WORKDIR "/src/src/ATI.Migrator"
RUN dotnet build "ATI.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ATI.Migrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATI.Migrator.dll", "-s"]
