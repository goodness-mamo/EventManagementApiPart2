# EventManagementApiPart2

# EventManagementApi

A RESTful Event Management API built with **ASP.NET Core 8, C#, Entity Framework Core, and SQL Server Express**. The application provides endpoints for managing events, organisers, participants, categories, enrolments, users, and results.

The project also includes a dedicated unit-testing project using **xUnit** and **Entity Framework Core InMemory** to verify controller behaviour, data operations, and business-rule validation.

## Table of Contents

- [Project Overview](#project-overview)
- [Key Features](#key-features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Configuration](#database-configuration)
- [Running the API](#running-the-api)
- [API Endpoints](#api-endpoints)
- [Unit Testing](#unit-testing)
- [Testing Strategy](#testing-strategy)
- [Error Handling and HTTP Status Codes](#error-handling-and-http-status-codes)
- [Future Improvements](#future-improvements)
- [Author](#author)

## Project Overview

EventManagementApi is designed to demonstrate the development and testing of a structured REST API using modern .NET technologies.

The API exposes HTTP endpoints that allow clients to create, retrieve, update, and delete event-management records. Entity Framework Core provides database access, while SQL Server Express stores persistent application data.

Automated unit tests are maintained separately from the main API to help verify expected behaviour and reduce regressions as the application evolves.

## Key Features

- RESTful API endpoints for event-management operations.
- Entity-based data modelling and database relationships.
- Entity Framework Core integration.
- SQL Server Express database support.
- Asynchronous service operations for event-related functionality.
- Controller-level CRUD operations.
- Validation of enrolment-related business rules.
- Handling of missing records and invalid requests.
- Automated unit tests using xUnit.
- In-memory test database support for isolated testing.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Application programming language |
| .NET 8 | Application runtime and framework |
| ASP.NET Core Web API | HTTP endpoints and request processing |
| Entity Framework Core | Object-relational mapping and data access |
| SQL Server Express | Persistent relational database |
| xUnit | Automated unit-testing framework |
| EF Core InMemory | Isolated test data storage |
| Visual Studio | Development, debugging, and test execution |
| Postman | Manual API request testing |

## Architecture

The application uses a controller-based API architecture with Entity Framework Core for database access. Event operations are delegated through an `IEventService` dependency.

```text
API Client / Postman
        |
        v
ASP.NET Core Routing
        |
        v
Controllers
        |
        +----------------------+
        |                      |
        v                      v
   AppDbContext           IEventService
        |                      |
        v                      v
 Entity Framework Core   Event Service Logic
        |
        v
 SQL Server Express
```

The unit tests exercise controller behaviour independently of a running web server. For controllers that use `AppDbContext` directly, the tests use an in-memory database. The service-based event controller requires a suitable test double or mock for `IEventService`.

## Project Structure

The solution contains the main API project and a separate unit-testing project.

```text
EventManagementApi/
│
├── EventManagementApi.sln
├── README.md
│
├── EventManagementApi/
│   ├── Controllers/
│   │   ├── UsersController.cs
│   │   ├── ParticipantsController.cs
│   │   ├── OrganisersController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   ├── ResultsController.cs
│   │   └── EventsController.cs
│   │
│   ├── Models/
│   │   └── Entities/
│   │
│   ├── DTOs/
│   │
│   ├── Data/
│   │   └── AppDbContext.cs
│   │
│   ├── Services/
│   │
│   ├── Properties/
│   │   └── launchSettings.json
│   │
│   ├── Program.cs
│   └── appsettings.json
│
└── EventManagementApi.Tests/
    ├── Helpers/
    │   └── TestDbContextFactory.cs
    ├── UsersControllerTests.cs
    ├── ParticipantsControllerTests.cs
    ├── OrganisersControllerTests.cs
    ├── CategoriesControllerTests.cs
    ├── EnrolmentsControllerTests.cs
    └── ResultsControllerTests.cs
```

*Note: This is a representative structure based on the project work. Keep only files and folders that actually exist in your repository, and add any additional test files as they are created.*

## Prerequisites

Before running the application, ensure you have:

- Visual Studio 2022 with the ASP.NET and web development workload.
- .NET 8 SDK.
- SQL Server Express.
- SQL Server Management Studio (SSMS), if you want a graphical database-management tool.
- Git for version control.
- Postman for manual API testing.

Verify the installed .NET SDK:

```bash
dotnet --list-sdks
```

Confirm that a .NET 8 SDK version is listed.

## Getting Started

### 1. Clone the repository

Replace the placeholder URL with your actual repository URL.

```bash
git clone <YOUR-REPOSITORY-URL>
cd EventManagementApi
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Build the solution

```bash
dotnet build
```

Resolve any compilation errors before proceeding.

## Database Configuration

The application uses SQL Server Express for persistent storage.

Update the connection string in `appsettings.json` to match your local SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EventManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Important:** This is an example configuration. The connection-string key, database name, server instance, and authentication method must match your actual application configuration.

For local development, Windows authentication is shown above. Use the appropriate authentication settings for your environment.

### Database migrations

If the project uses Entity Framework Core migrations, apply them using the appropriate startup project and migration configuration.

For example:

```bash
dotnet ef database update --project EventManagementApi --startup-project EventManagementApi
```

If the EF Core CLI is not installed, install it using the official .NET tooling instructions. Ensure the required design-time package is available and that the command matches your solution structure.

Do not run migrations until the connection string and database configuration have been verified.

## Running the API

From the solution directory:

```bash
dotnet run --project EventManagementApi
```

Alternatively, open the solution in Visual Studio, select `EventManagementApi` as the startup project, and click **Run**.

The application will display its local listening URLs in the console or Visual Studio output window.

Use the configured URL when sending requests from Postman. If Swagger/OpenAPI is configured and enabled for your development environment, you can also use its UI to explore the endpoints.

## API Endpoints

The following table summarises the API resource areas developed in the project. Confirm the exact route prefixes and action names in the controller attributes before using these paths.

| Resource | Typical operations |
|---|---|
| Users | Retrieve users and retrieve a user by ID |
| Events | Retrieve, create, update, and delete events |
| Organisers | Manage organiser records |
| Participants | Manage participant records |
| Categories | Manage event categories |
| Enrolments | Manage enrolments and validate related records |
| Results | Retrieve result records |

Typical REST conventions include:

| Method | Purpose |
|---|---|
| `GET` | Retrieve resources |
| `POST` | Create a resource |
| `PUT` | Update a resource |
| `DELETE` | Delete a resource |

Actual endpoint availability and route formats depend on the controller implementation.

### Example request

```http
GET /api/participants
```

A successful retrieval commonly returns `200 OK` with a JSON response body.

Use Postman to verify the actual endpoint URLs, request bodies, response payloads, and status codes.

## Unit Testing

The solution includes a separate test project:

`EventManagementApi.Tests`

The project uses xUnit and Entity Framework Core InMemory to test controller behaviour without requiring a live SQL Server database for every unit test.

### Test dependencies

The test project uses:

- xUnit for test definitions and assertions.
- `Microsoft.EntityFrameworkCore.InMemory` for isolated test data.
- A reusable `TestDbContextFactory` helper for creating test database contexts.
- Visual Studio Test Explorer or the .NET CLI to execute tests.

### Test project structure

```text
EventManagementApi.Tests/
│
├── Helpers/
│   └── TestDbContextFactory.cs
│
├── UsersControllerTests.cs
├── ParticipantsControllerTests.cs
├── CategoriesControllerTests.cs
├── EnrolmentsControllerTests.cs
├── ResultsControllerTests.cs
└── OrganisersControllerTests.cs
```

### Running unit tests

Run all tests from the solution directory:

```bash
dotnet test
```

To build the solution and run its tests separately:

```bash
dotnet build
dotnet test
```

Alternatively, open **Test Explorer** in Visual Studio and select **Run All Tests**.

### Controller test coverage

| Test class | Areas covered |
|---|---|
| `UsersControllerTests` | Retrieving users, retrieving by ID, missing records |
| `ParticipantsControllerTests` | Participant operations and controller responses |
| `CategoriesControllerTests` | CRUD operations, invalid IDs, missing records |
| `EnrolmentsControllerTests` | CRUD operations, relationship validation, duplicate enrolments |
| `ResultsControllerTests` | Retrieving results and missing-record handling |
| `OrganisersControllerTests` | CRUD operations, updates, deletion and missing records |

The Events controller requires separate testing because it depends on `IEventService`. Its test double and test results should be verified before its coverage is reported as complete.

## Testing Strategy

The unit tests follow the **Arrange–Act–Assert (AAA)** pattern.

1. **Arrange:** Set up the test data, context, and controller.
2. **Act:** Execute the controller method.
3. **Assert:** Verify the action result, status code, returned data, or changes to the test database.

The testing strategy includes:

- Successful requests.
- Retrieval of existing records.
- Missing-record scenarios.
- Invalid input and mismatched IDs.
- Successful creation, updates, and deletions.
- Enrolment relationship validation.
- Duplicate enrolment handling.
- Verification of expected HTTP action results.

### Enrolment business rules

The enrolment tests cover important validation scenarios:

| Scenario | Expected behaviour |
|---|---|
| Invalid event/category relationship | Return `400 Bad Request` |
| Participant does not exist | Return `400 Bad Request` |
| Duplicate enrolment | Return `409 Conflict` |
| Valid enrolment | Create the enrolment and populate `EnrolledAt` where implemented |

### Important testing limitation

The EF Core InMemory provider is useful for unit testing, but it does not reproduce every SQL Server behaviour. Integration tests against a dedicated test database are recommended for database constraints, provider-specific queries, migrations, and complete HTTP request handling.

## Error Handling and HTTP Status Codes

The API uses HTTP action results to communicate request outcomes.

| Status code | Meaning |
|---|---|
| `200 OK` | The request succeeded |
| `201 Created` | A resource was created |
| `204 No Content` | The operation succeeded without a response body |
| `400 Bad Request` | The request or input is invalid |
| `404 Not Found` | The requested resource does not exist |
| `409 Conflict` | The request conflicts with existing data or a constraint |

The exact status code depends on the action and the validation rules implemented by the controller.

## Future Improvements

Potential next steps include:

- Complete and verify the Events controller unit tests.
- Expand coverage for invalid input and edge cases.
- Add integration tests for API routing, model binding, and SQL Server behaviour.
- Add API documentation with Swagger/OpenAPI if not already configured.
- Introduce automated test execution in a CI/CD pipeline.
- Add code coverage reporting.
- Consider containerising the application for repeatable deployment.
- Document the actual endpoint URLs and example request/response payloads.

## Author

**Project:** EventManagementApi  
**Framework:** ASP.NET Core 8  
**Language:** C#  
**Testing:** xUnit and Entity Framework Core InMemory

This project demonstrates API development, relational data access, controller testing, validation, and automated quality assurance using the .NET ecosystem.
