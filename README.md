# Vertical Slice Architecture Demo

A demonstration of **Vertical Slice Architecture** in ASP.NET Core 10, organizing functionality around features rather than technical layers. Each feature is a self-contained slice that owns its full pipeline — from HTTP endpoint to database.

## Architecture Overview

Instead of grouping code by technical role (Controllers, Services, Repositories), this project groups code **by feature**. Each operation lives in its own folder with everything it needs:

```
Features/
└── Todos/
    └── CreateTodo/
        ├── Endpoint.cs    # Route definition, Request/Response types
        ├── Handler.cs     # Business logic
        └── Validator.cs   # Input validation rules
```

This means adding a new feature requires touching only one folder. Changing a feature's behavior doesn't ripple across multiple layers.

## Project Structure

```
src/
├── Demo.Domain/           # Entities and enums — no framework dependencies
├── Demo.Application/      # Feature slices and shared interfaces
├── Demo.Infrastructure/   # EF Core, PostgreSQL, repository implementations
└── Demo.API/              # Minimal API setup, middleware, DI wiring
```

### Demo.Domain

Pure domain entities with encapsulated behavior. No persistence or framework concerns.

| Entity | Key Behavior |
|--------|-------------|
| `Todo` | `Update()`, `MarkAsCompleted()`, `AddSteps()`, `AddTags()` |
| `Step` | `MarkAsCompleted()`, `MarkAsIncomplete()`, `UpdateOrder()` |
| `Tag`  | `ChangeName()` |

All entities inherit from `Entity`, which provides a `Guid` (v7) primary key and `CreatedAt`/`UpdatedAt` timestamps.

### Demo.Application

Contains the feature slices and the interfaces they depend on:

- `IEndpoint` — implemented by every `Endpoint.cs`; auto-discovered and registered at startup
- `IHandler<TRequest, TResult>` — generic handler returning Minimal API `IResult` types
- `IRepository<T>` / `ITodoRepository` / `ITagRepository` / `IStepRepository` — data access contracts
- `IUnitOfWork` — transaction boundary

**Features:**

| Domain | Operation | HTTP |
|--------|-----------|------|
| Todos | CreateTodo | `POST /todos` |
| Todos | GetTodos | `GET /todos` |
| Todos | GetTodoById | `GET /todos/{id}` |
| Todos | ChangeTodo | `PUT /todos/{id}` |
| Todos | DeleteTodo | `DELETE /todos/{id}` |
| Tags | CreateTag | `POST /tags` |
| Tags | GetTags | `GET /tags` |
| Tags | ChangeTag | `PUT /tags/{id}` |
| Steps | CreateStep | `POST /steps` |
| Steps | GetSteps | `GET /steps` |
| Steps | ChangeStep | `PUT /steps/{id}` |
| Steps | DeleteStep | `DELETE /steps/{id}` |

### Demo.Infrastructure

EF Core + PostgreSQL implementation of the persistence interfaces defined in Demo.Application.

- `ApplicationDbContext` — implements both `DbContext` and `IUnitOfWork`
- Generic `Repository<T>` base class with specialized overrides (e.g. `TodoRepository` eager-loads Steps and Tags)
- Entity configurations using field-access mode to respect domain encapsulation (`_steps`, `_tags` private backing fields)
- Snake_case column naming via `EFCore.NamingConventions`

### Demo.API

Startup wiring and cross-cutting concerns. Features self-register — no manual endpoint registration.

- **Auto-discovery**: Scans assemblies for `IEndpoint` and `IHandler<,>` implementations and registers them
- **Validation filter**: Applied per-endpoint; resolves the matching FluentValidation validator and returns `400 ValidationProblem` on failure
- **Global exception handler**: Maps `ValidationException` → 400, everything else → 500 with `ProblemDetails`
- **API docs**: Scalar UI served at `/scalar/v1`

## Tech Stack

| Concern | Library |
|---------|---------|
| Web framework | ASP.NET Core 10 Minimal APIs |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL 18 |
| Validation | FluentValidation 12 |
| Mapping | Mapster 10 |
| API docs | Scalar + Microsoft.AspNetCore.OpenApi |

## Running Locally

**Prerequisites:** .NET 10 SDK, Docker

```bash
# Start PostgreSQL
docker compose up -d

# Run the API
dotnet run --project src/Demo.API
```

API docs are available at `https://localhost:{port}/scalar/v1`.

## Key Design Decisions

**Why vertical slices over layered architecture?**
Layered architecture couples unrelated features through shared layers. A change to `TodoService` can affect unrelated features that share the same service or base class. Vertical slices eliminate that coupling — each feature is independently changeable.

**Why Minimal APIs instead of controllers?**
Minimal APIs reduce ceremony and make the per-feature pattern explicit. Each `Endpoint.cs` is a small, focused file that defines the contract and delegates to a handler.

**Why generic `IHandler<TRequest, TResult>` instead of MediatR?**
The handler interface is simple enough to implement directly, avoiding the overhead and indirection of a mediator. Handlers are injected directly into endpoints, keeping the call stack shallow and debuggable.

**Why repository + unit of work over direct DbContext usage?**
The interfaces in Demo.Application keep the feature slices independent of the EF Core implementation, making the persistence layer swappable and the handlers easier to test.
