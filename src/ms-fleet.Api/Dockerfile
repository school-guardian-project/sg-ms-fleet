FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ms-fleet.Api/ms-fleet.Api.csproj", "src/ms-fleet.Api/"]
RUN dotnet restore "src/ms-fleet.Api/ms-fleet.Api.csproj"
COPY . .
WORKDIR "/src/src/ms-fleet.Api"
RUN dotnet build "./ms-fleet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ms-fleet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ms-fleet.Api.dll"]
