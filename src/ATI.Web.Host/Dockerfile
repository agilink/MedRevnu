#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ATI.Web.Host/ATI.Web.Host.csproj", "src/ATI.Web.Host/"]
COPY ["src/ATI.Web.Core/ATI.Web.Core.csproj", "src/ATI.Web.Core/"]
COPY ["src/ATI.Application/ATI.Application.csproj", "src/ATI.Application/"]
COPY ["src/ATI.Application.Shared/ATI.Application.Shared.csproj", "src/ATI.Application.Shared/"]
COPY ["src/ATI.Core.Shared/ATI.Core.Shared.csproj", "src/ATI.Core.Shared/"]
COPY ["src/ATI.Core/ATI.Core.csproj", "src/ATI.Core/"]
COPY ["src/ATI.EntityFrameworkCore/ATI.EntityFrameworkCore.csproj", "src/ATI.EntityFrameworkCore/"]
COPY ["src/ATI.GraphQL/ATI.GraphQL.csproj", "src/ATI.GraphQL/"]
RUN dotnet restore "src/ATI.Web.Host/ATI.Web.Host.csproj"
COPY . .
WORKDIR "/src/src/ATI.Web.Host"
RUN dotnet build "ATI.Web.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ATI.Web.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATI.Web.Host.dll"]
