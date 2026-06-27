# Product API — RESTful Backend Assessment

A Clean Architecture RESTful API for managing **Products** and their related **Items**, built with ASP.NET Core 8, EF Core, and SQL Server.

## Architecture

```
ProductApi.Domain          -> Entities (Product, Item), domain exceptions. No dependencies.
ProductApi.Application     -> DTOs, service interfaces, services (business logic), FluentValidation validators.
ProductApi.Infrastructure  -> EF Core DbContext, Repository implementations, JWT TokenService.
ProductApi.API             -> Controllers, middleware, Program.cs (composition root).
ProductApi.Tests           -> xUnit + Moq unit tests for the service layer.
```

**Flow:** `Controller -> Service (business rules) -> Repository (EF Core) -> SQL Server`

This keeps each layer responsible for one thing only — controllers don't know about EF Core, and the database can be swapped without touching business logic.

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- EF Core + SQL Server
- JWT Bearer authentication
- FluentValidation
- Swagger / OpenAPI
- xUnit + Moq
- Docker + Docker Compose

## Database Schema

Matches the schema given in the assessment exactly:

```sql
CREATE TABLE [dbo].[Product] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductName] NVARCHAR(255) NOT NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [ModifiedBy] NVARCHAR(100) NULL,
    [ModifiedOn] DATETIME NULL
)

CREATE TABLE [dbo].[Item] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [ProductId] INT NOT NULL FOREIGN KEY REFERENCES Product(Id),
    [Quantity] INT NOT NULL
)
```

EF Core migrations create these tables automatically — no manual SQL needed.

## Running Locally (SQL Server Express)

1. Update the connection string in `src/ProductApi.API/appsettings.Development.json` if your SQL Server instance name/credentials differ from the default (`DESKTOP-NAPEB9N\SQLEXPRESS`, `sa/sa123`).

2. Restore and build:
   ```bash
   cd ProductApi.Solution
   dotnet restore
   dotnet build
   ```

3. Install the EF Core tools (one-time, if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. Create the database via migrations:
   ```bash
   cd src/ProductApi.API
   dotnet ef migrations add InitialCreate --project ../ProductApi.Infrastructure --startup-project .
   dotnet ef database update --project ../ProductApi.Infrastructure --startup-project .
   ```

5. Run the API:
   ```bash
   dotnet run
   ```

6. Open Swagger UI: **https://localhost:{port}/swagger** (port shown in console on startup). Click "Authorize", paste the JWT from `/api/v1/auth/login` as `Bearer {token}`, and all write endpoints become testable.

## Running via Docker

```bash
cd ProductApi.Solution
docker-compose up --build
```

This spins up SQL Server + the API together. API will be available at `http://localhost:8080/swagger`.
> Note: when using Docker, run EF migrations once against the containerized DB the same way as step 4 above, pointing the connection string at `localhost,1433`.

## Authentication

This is a CRUD assessment, so auth is intentionally simple — there's no Users table.

`POST /api/v1/auth/login` with any non-empty username/password returns a JWT. Use that token as `Authorization: Bearer {token}` on all write endpoints (`POST`, `PUT`, `DELETE`). `GET` endpoints are public.

## API Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/auth/login` | No | Get a JWT token |
| GET | `/api/v1/products?pageNumber=1&pageSize=10` | No | List products (paginated) |
| GET | `/api/v1/products/{id}` | No | Get a single product |
| GET | `/api/v1/products/{id}/items` | No | Get items for a product (related resource) |
| POST | `/api/v1/products` | Yes | Create a product |
| PUT | `/api/v1/products/{id}` | Yes | Update a product |
| DELETE | `/api/v1/products/{id}` | Yes | Delete a product |
| POST | `/api/v1/items` | Yes | Create an item under a product |

## Running Tests

```bash
cd tests/ProductApi.Tests
dotnet test
```

## Notes on Scope

This implementation covers every section listed in the assessment brief (auth, error-handling middleware, FluentValidation, versioned routes, service layer, repository pattern, EF Core, pagination, `AsNoTracking()`, async throughout, CORS, Swagger docs, Docker) while staying intentionally simple — no over-engineered abstractions, no unused folders, no speculative features beyond what Products/Items CRUD needs.
