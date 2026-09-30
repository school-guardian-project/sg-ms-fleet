# sg-ms-fleet

Microservicio de gestión de flota - School Guardian.

## Stack

- .NET 10
- EF Core 10 + SQL Server
- Clean Architecture + Ports & Adapters
- Controllers + OpenAPI + Scalar
- gRPC
- Kafka
- AutoMapper
- xUnit

## Estructura

```
src/ms-fleet.Api/
├── Driver/          (bounded context - choferes)
├── Vehicle/         (bounded context - vehículos)
├── Shared/          (shared kernel)
├── Infrastructure/  (DI, Kafka)
└── Program.cs

tests/ms-fleet.Tests/
```

## Comandos

```bash
dotnet restore
dotnet build
dotnet test
docker compose up -d
```
