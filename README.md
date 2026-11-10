# Integra Partners API

A small ASP.NET Core Web API for managing employees, built as a technical exercise. It exposes CRUD endpoints secured with an API key and stores data in a local JSON file.

## Tech stack

- .NET 10 / ASP.NET Core Web API (controllers)
- `System.Text.Json` for persistence to `Data/employees.json`
- Built-in dependency injection, ProblemDetails error responses, and OpenAPI document generation

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Configure the API key

The API key is **not** stored in source control. Set it with user secrets for local development:

```bash
cd IntegraPartnersApi
dotnet user-secrets set ApiKey "<your-key-here>"
```

In other environments, set it as an environment variable named `ApiKey`.

### Run

```bash
dotnet run --project IntegraPartnersApi
```

The API listens on:

- `http://localhost:5041`
- `https://localhost:7190` (with the `https` launch profile)

In Development, the OpenAPI document is available at `/openapi/v1.json`.

## Authentication

Every request must include the API key in the `X-API-KEY` header:

```
X-API-KEY: <your-key-here>
```

A missing or invalid key returns `401 Unauthorized`.

## Endpoints

Base route: `/api/Employee`

| Method | Route                | Description              | Success          | Errors                          |
|--------|----------------------|--------------------------|------------------|---------------------------------|
| GET    | `/api/Employee`      | List all employees       | `200 OK`         | `401`                           |
| GET    | `/api/Employee/{id}` | Get one employee         | `200 OK`         | `401`, `404 Not Found`          |
| POST   | `/api/Employee`      | Create an employee       | `201 Created`    | `401`, `409 Conflict` (email in use) |
| PUT    | `/api/Employee/{id}` | Update an employee       | `204 No Content` | `401`, `404`, `409` (email in use)   |
| DELETE | `/api/Employee/{id}` | Delete an employee       | `204 No Content` | `401`, `404`                    |

Unhandled server errors return `500` with a standard [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) body. Exception details are never sent to the client.

### Employee model

```json
{
  "id": 1,
  "firstName": "James",
  "lastName": "Anderson",
  "email": "james.ander@example.com"
}
```

`id` is assigned by the server on create and taken from the route on update.

### Examples

Create an employee:

```bash
curl -X POST http://localhost:5041/api/Employee \
  -H "X-API-KEY: <your-key-here>" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Jane","lastName":"Doe","email":"jane.doe@example.com"}'
```

Update an employee:

```bash
curl -X PUT http://localhost:5041/api/Employee/1 \
  -H "X-API-KEY: <your-key-here>" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"James","lastName":"Anderson","email":"james.anderson@example.com"}'
```

Delete an employee:

```bash
curl -X DELETE http://localhost:5041/api/Employee/1 -H "X-API-KEY: <your-key-here>"
```

## Project structure

```
IntegraPartnersApi/
├── Controllers/
│   └── EmployeeController.cs      # HTTP endpoints; depends only on IEmployeeRepository
├── Data/
│   ├── IEmployeeRepository.cs     # Storage abstraction
│   ├── JsonEmployeeRepository.cs  # JSON-file implementation
│   └── employees.json             # Seed / persisted data
├── Models/
│   └── Employee.cs
├── Utility/
│   └── ApiKeyAttribute.cs         # X-API-KEY action filter
└── Program.cs                     # Service registration and middleware pipeline
```

## Design decisions

- **Repository pattern with DI.** The controller depends on `IEmployeeRepository`, not on a concrete storage class. Swapping the JSON file for a database means writing a new implementation and changing a single registration line in `Program.cs`. It also lets the controller be unit tested with a mocked repository.
- **Singleton repository with a file lock.** `JsonEmployeeRepository` is registered as a singleton and serializes all reads and writes through a `SemaphoreSlim`. Concurrent requests therefore can't corrupt the file, lose updates, or assign duplicate IDs.
- **RESTful semantics.** Create and update are separate verbs (`POST` / `PUT`), with correct status codes (`201` with a `Location` header, `204`, `404`, `409`).
- **Centralized error handling.** `UseExceptionHandler` and `AddProblemDetails` replace per-action try/catch blocks, so clients get consistent error bodies without internal details.
- **Secrets out of source control.** The API key comes from configuration (user secrets locally, environment variables elsewhere).
- **Case-insensitive email uniqueness** on both create and update.

## Future improvements

With more time, I would add:

- **Validation:** `[Required]` / `[EmailAddress]` attributes (or FluentValidation) on the incoming model.
- **Request/response DTOs**, separate from the stored entity.
- **Unit tests** for the controller (mocked repository) and **integration tests** with `WebApplicationFactory`.
- **A real database** (e.g. EF Core with SQL Server or SQLite), with a unique index on email to enforce uniqueness atomically.
- **Structured logging** and correlation IDs.
- **Swagger UI / Scalar** with the API key defined as a security scheme.
- **Pagination and filtering** on `GET /api/Employee`.
- **Stronger auth** (per-client keys or OAuth2/JWT) if this were to go beyond an exercise.
