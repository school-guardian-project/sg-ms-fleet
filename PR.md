# PR: feat(fleet) — Bus Management with Hexagonal Architecture

## Summary

Implements **HU-FLEET-001** (bus fleet CRUD) and **HU-BUS-002** (assign driver to bus) for the `ms-fleet` microservice, following the hexagonal architecture pattern defined in `docs/_stacks/dotnet-aspnet.md`.

## Changes

### Domain Layer
- `Domain/Model/Bus.cs` — Bus entity with invariants (plate, capacity, status)
- `Domain/Model/Brand.cs` — Brand catalog entity
- `Domain/Model/Model.cs` — Model catalog entity (linked to Brand)
- `Domain/Model/DriverAssignment.cs` — Driver-to-bus assignment entity
- `Domain/Model/Status.cs` — Status enum (Active, Inactive)
- `Domain/Ports/In/` — 7 use case interfaces (Create, Update, ChangeStatus, Get, List, AssignDriver, UnassignDriver)
- `Domain/Ports/Out/` — 4 output ports (IBusRepository, IDriverAssignmentRepository, IIamService, IGpsDeviceService)

### Application Layer
- `Application/UseCase/CreateBusService.cs` — Creates bus with plate uniqueness + GPS validation
- `Application/UseCase/UpdateBusService.cs` — Updates bus data
- `Application/UseCase/ChangeBusStatusService.cs` — Activates/deactivates bus
- `Application/UseCase/GetBusService.cs` — Gets bus by ID
- `Application/UseCase/ListBusesService.cs` — Lists all buses
- `Application/UseCase/AssignDriverToBusService.cs` — Assigns driver via IAM profile validation
- `Application/UseCase/UnassignDriverFromBusService.cs` — Unassigns driver from bus

### Infrastructure Layer
- `Infrastructure/Configuration/BusConfiguration.cs` — EF Core config for Bus
- `Infrastructure/Configuration/BrandConfiguration.cs` — EF Core config for Brand
- `Infrastructure/Configuration/ModelConfiguration.cs` — EF Core config for Model
- `Infrastructure/Configuration/DriverAssignmentConfiguration.cs` — EF Core config for DriverAssignment
- `Infrastructure/Controller/BusController.cs` — REST API controller
- `Infrastructure/Persistence/Context/FleetContext.cs` — DbContext (schema: Fleet)
- `Infrastructure/Persistence/Entity/` — BusEntity, BrandEntity, ModelEntity, DriverAssignmentEntity
- `Infrastructure/Persistence/Repository/BusRepository.cs` — IBusRepository implementation
- `Infrastructure/Persistence/Repository/DriverAssignmentRepository.cs` — IDriverAssignmentRepository implementation
- `Infrastructure/External/IamService.cs` — HTTP client to IAM service for profile validation
- `Infrastructure/External/GpsDeviceService.cs` — HTTP client to GPS tracking service (simulated)
- `Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` — DI wiring

### Tests
- `tests/ms-fleet.Tests/Domain/BusTests.cs` — 2 unit tests
- `tests/ms-fleet.Tests/Application/CreateBusServiceTests.cs` — 3 unit tests
- `tests/ms-fleet.Tests/Application/AssignDriverToBusServiceTests.cs` — 3 unit tests
- `tests/ms-fleet.Tests/Fakes/` — 4 in-memory fakes

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/buses` | Create bus |
| PUT | `/api/buses/{id}` | Update bus |
| PATCH | `/api/buses/{id}/status` | Change bus status |
| GET | `/api/buses` | List buses |
| GET | `/api/buses/{id}` | Get bus by ID |
| PUT | `/api/buses/{busId}/driver` | Assign driver to bus |
| DELETE | `/api/buses/{busId}/driver` | Unassign driver from bus |

## Database Schema (Fleet)

| Table | Description |
|-------|-------------|
| Bus | Fleet vehicles |
| Brand | Vehicle brands |
| Model | Vehicle models (linked to Brand) |
| DriverAssignemts | Driver-to-bus assignments |

## User Stories

- **HU-FLEET-001** — Administrator manages the bus fleet (create, edit, activate/deactivate)
- **HU-BUS-002** — Administrator assigns a driver to bus

## Test Plan

- [x] Domain unit tests (Bus entity invariants)
- [x] Application unit tests (CreateBusService, AssignDriverToBusService)
- [ ] Integration tests (pending)
- [ ] E2E tests (pending)

## Checklist

- [x] Hexagonal architecture followed
- [x] Domain has no external dependencies
- [x] Configuration separated from context
- [x] Tests with in-memory fakes
- [x] No hardcoded URLs
- [x] No Kafka (only HTTP + gRPC ready)
- [ ] CI pipeline green
- [ ] Code review approved

## Related

- Branch: `feature/fleet-bus-management`
- Base: `dev`
- Commits: `6e7309e`, `9752db9`
