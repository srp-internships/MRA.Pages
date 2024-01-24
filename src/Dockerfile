#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 9595

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MRA.Pages.Api/MRA.Pages.Api.csproj", "MRA.Pages.Api/"]
COPY ["MRA.Pages.Application/MRA.Pages.Application.csproj", "MRA.Pages.Application/"]
COPY ["MRA.Pages.Application.Contract/MRA.Pages.Application.Contract.csproj", "MRA.Pages.Application.Contract/"]
COPY ["MRA.Pages.Domain/MRA.Pages.Domain.csproj", "MRA.Pages.Domain/"]
COPY ["MRA.Pages.Infrastructure/MRA.Pages.Infrastructure.csproj", "MRA.Pages.Infrastructure/"]
RUN dotnet restore "./MRA.Pages.Api/MRA.Pages.Api.csproj"
COPY . .
WORKDIR "/src/MRA.Pages.Api"
RUN dotnet build "./MRA.Pages.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MRA.Pages.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MRA.Pages.Api.dll"]