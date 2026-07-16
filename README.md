# Transactions Backend API

A sample ASP.NET Core 8 Web API demonstrating **JWT authentication**, **role-based authorization** (Admin / User), and **CRUD endpoints** for user profiles and financial transactions.

## Project Structure

```
TransactionsBackend/
├── Constants/          # Role names
├── Controllers/        # HTTP endpoints (Auth, Users, Transactions)
├── Data/               # EF Core DbContext and seed data
├── Models/
│   ├── Dtos/           # Request/response contracts
│   └── Entities/       # User, Transaction
├── Security/           # JWT settings and token generator
└── Services/           # Business logic (+ Interfaces/)
```

## Requirements Covered

| Requirement | Implementation |
|---|---|
| 2 CRUD APIs | `UsersController`, `TransactionsController` |
| Controller → Service | Controllers delegate to `IUserService`, `ITransactionService` |
| Data models | `User`, `Transaction` entities + DTOs |
| Async methods | All service and controller actions are `async` |
| JWT security | Bearer token auth via `JwtBearer` middleware |
| Admin vs User rights | `[Authorize(Roles = ...)]` + service-level checks |

## Role Access Matrix

| Endpoint | Admin | User |
|---|---|---|
| `POST /api/auth/login` | ✓ (public) | ✓ (public) |
| `GET /api/users` | All users | Own profile only |
| `GET /api/users/{id}` | Any user | Own profile only |
| `POST /api/users` | ✓ | ✗ |
| `PUT /api/users/{id}` | Any user | Own profile only |
| `DELETE /api/users/{id}` | ✓ | ✗ |
| `GET /api/transactions` | All | Own only |
| `GET /api/transactions/{id}` | Any | Own only |
| `POST /api/transactions` | Any user (via `UserId`) | Own only |
| `PUT /api/transactions/{id}` | Any | Own only |
| `DELETE /api/transactions/{id}` | Any | Own only |

## Demo Accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@transactions.demo` | `Admin123!` |
| User | `user@transactions.demo` | `User123!` |

## Run Locally

```bash
cd TransactionsBackend
dotnet restore
dotnet run --project TransactionsBackend
```

Open Swagger UI: `https://localhost:7xxx/swagger` (port shown in terminal).

### Quick Test Flow

1. **Login** — `POST /api/auth/login` with demo credentials.
2. Copy the `token` from the response.
3. In Swagger, click **Authorize** and enter: `Bearer {token}`
4. Call protected endpoints (`/api/users`, `/api/transactions`).

### Example Login (curl)

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"user@transactions.demo\",\"password\":\"User123!\"}"
```

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core (In-Memory provider)
- JWT Bearer authentication
- BCrypt password hashing
- Swagger / OpenAPI
