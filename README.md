# Meridian Ordering C# Starter

This public repository is a guided starting point for developers implementing Meridian Ordering from the supplied business and technical requirements. It provides a buildable .NET solution, local infrastructure, and high-level responsibility boundaries. It is **not** the completed implementation.

## Prerequisites

- .NET SDK version selected by `global.json`
- Docker Desktop or another Docker Engine with Docker Compose
- Git

## Local infrastructure

Create a local environment file from the safe disposable template:

```powershell
Copy-Item .env.example .env
```

Start SQL Server and RabbitMQ:

```powershell
docker compose up -d order-store rabbitmq
```

Start the optional Azure Service Bus emulator profile as well:

```powershell
docker compose --profile service-bus up -d
```

Stop and remove the containers:

```powershell
docker compose --profile service-bus down
```

The example credentials are disposable local-development values and must not be reused outside a local workstation. Application database objects, queues, topics, subscriptions, and other application-owned resources are intentionally not provisioned by this starter.

## Build

```powershell
dotnet build MeridianOrdering.sln
```

The empty starter test project deliberately contains no scenarios. Microsoft Testing Platform therefore reports zero tests and exits with code 8 until the first real acceptance scenario is added; see the [Acceptance README](tests/MeridianOrdering.Tests/Acceptance/README.md).

## Project map

| Path | Purpose |
| --- | --- |
| `docs/requirements/` | Authoritative business and technical training inputs |
| `docs/implementation-guide.md` | Recommended implementation and decision-making workflow |
| `src/MeridianOrdering/DomainFacade.cs` | Named public application boundary |
| `src/MeridianOrdering/Managers/` | Customer and ordering coordination, validation, gateways, messaging, and composition shells |
| `src/MeridianOrdering/Managers/DataLayer/DataManagers/` | Persistence coordination shells inside the data-layer boundary |
| `src/MeridianOrdering.Api/` | Startup-safe ASP.NET Core host and empty capability-specific controller shells |
| `tests/MeridianOrdering.Tests/Acceptance/` | Functional acceptance scenarios written by students |
| `infrastructure/` | Application-owned local provisioning placeholders |

## Begin here

Read all documents under `docs/requirements/`, then read the [implementation guide](docs/implementation-guide.md). Identify the public application boundary and write one functional acceptance scenario for a happy-path vertical slice. Implement only enough behavior to satisfy that scenario before adding refusal, failure, persistence, messaging, rollback, and nuance coverage.

The starter deliberately contains no completed business behavior, application schema, stored procedures, acceptance scenarios, or test-support solution code.
