# Servicio Fleet

Fleet management microservice for School Guardian. Manages buses, vehicle types, brands, and models for the school transportation system. Built with .NET 10 following Clean Architecture and Ports & Adapters patterns.

## Getting Started

These instructions will give you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on deploying the project on a live system.

### Prerequisites

Requirements for the software and other tools to build, test and push:

- .NET 10 SDK
- Docker
- Docker Compose
- The `sg-services-network` Docker network (external, shared across services)

### Installing

A step by step series of examples that tell you how to get a development environment running.

Create the external network if it does not exist:

    docker network create sg-services-network

Start the service:

    docker compose up -d

The API listens on HTTP port 8080 and exposes an internal gRPC endpoint (h2c) on port 5001 for consumption by `ms-route`. Environment variables are loaded from `.env`.

## Running the tests

Explain how to run the automated tests for this system.

### Sample Tests

Tests are executed against the test service that runs in Docker Compose for each service:

    docker compose --profile tests run ms-fleet.tests

### Style test

Checks if the best practices and the right coding style has been used, validated through the test service in Docker Compose.

## Deployment

Deployment is performed via GitHub Actions to an AWS EC2 instance.

## Built With

- .NET 10
- EF Core 10 + SQL Server
- Clean Architecture + Ports & Adapters
- Controllers + OpenAPI + Scalar
- gRPC
- Kafka
- AutoMapper
- xUnit

## Contributing

Please read [CONTRIBUTING.md](https://github.com/school-guardian-project/.github/blob/main/CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the SCHOOL GUARDIAN PROPRIETARY SOFTWARE LICENSE Version 1.0 — 2026. See the [LICENSE](LICENSE) file for details.
