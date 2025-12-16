# Autistic Bank

## Coursework for "Databases" course

A backend application for banking system automation designed with a multi-layered architecture (Clean Architecture) to ensure scalability and maintainability. The system manages accounts, card issuance, loan lifecycles, and secure transactions, while keeping strict separation between business logic and infrastructure.

## Worked on project:

- Легеза Данііл Павлович ІМ-41
- Бойко Данило Сергійович ІМ-41

---

## Tech stack 

- **Backend:** C# 13 (.NET 9.0)
- **Framework:** ASP.NET Core Web API
- **Database:** PostgreSQL 14
- **ORM:** Entity Framework Core 9.0 (EF Core)
- **Testing:** xUnit, FluentAssertions, EF Core InMemory

#### **Additional libraries:**

- **BCrypt.Net:** For secure password hashing
- **CSharpFunctionalExtensions:** To handle Result patterns and avoid exceptions
- **Swagger:** For API documentation and testing UI

---

## Configuration Guide

### Prerequisites

Make sure you have the following installed:

- [**Docker Desktop**](https://docs.docker.com/desktop/)
- [**.NET SDK**](https://learn.microsoft.com/en-us/dotnet/core/install/)
- [**Git**](https://git-scm.com/install/)

---

## 📦 Project Setup

### Step 1. Clone the Repository

```bash
git clone https://github.com/d1syax/database-bank.git
cd database-bank
```

### Step 2. Configure Environment Settings

Change YOUR_PASSWORD_HERE for whatever you want in docker-compose.yml && /MyBank.Api/appsetings.json

```yml
POSTGRES_PASSWORD: YOUR_PASSWORD_HERE
```

```C#
"DefaultConnection": "Host=localhost;Port=5432;Database=coursework;Username=postgres;Password=YOUR_PASSWORD_HERE"
```

### Step 3. Start Database Services

```bash
docker-compose up -d 
```

Afterwards on localhost:5050, you can use pgAdmin<br>
**Authorization credentials:**

**Login:** admin@admin.com<br>
**Password:** root<br>

Then register server with
```
Hostname/address: mybank_postgres
Port: 5432
Maintenance database: postgres
Username: postgres
Password: YOUR_PASSWORD_HERE
```

### Step 4. Apply Database Migrations
Run Entity Framework Core migrations to create database schema:
```bash
dotnet ef database update \
  --project MyBank.Infrastructure \
  --startup-project MyBank.Api
```

### Step 5. Run the Application
```bash
dotnet run --project MyBank.Api
```
The API will be available at: https:localhost:3000<br>
To interactively explore and test the endpoints, navigate to: https://localhost:3000/swagger

## Tests:
Firstly, navigate to the folder containing tests **/MyBank.Tests**<br>

**Start all of the tests**

```bash
dotnet test
```

**Run a Specific Test Class**

```bash
dotnet test --filter <name> # For instance(AccountServiceTests)
```
## Structure of Project
```
MyBank/
├── MyBank.Api/                       # Presentation Layer (Enttry point)
│   ├── Controllers/                  # API controllers
│   ├── Middleware/                   # (Error catches, etc..)
│   ├── appsettings.json              # Connection to DB
│   └── Program.cs                    # Config, DI Container, start of pipeline 
│
├── MyBank.Application/               # Application Layer
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Analytics/                # For analytical queries
│   │   ├── Requests/                 # Requests from user
│   │   └── Responses/                # Responses to user
│   └── Services/                     # Logic of application
│       └── ...                     
│
├── MyBank.Domain/                    # Domain Layer
│   ├── Common/                       # General classes (BaseEntity, SoftDeletableEntity)
│   ├── Constants/                    # Constants
│   ├── Entities/                     # Entities for DB (User, Transaction)
│   ├── Enums/                        # Enums (AccountType, Status)
│   └── Interfaces/                   # Abstractions
│
├── MyBank.Infrastructure/            # Infrastructure Layer
│   ├── Migrations/                   # Файли міграцій EF Core (історія змін БД)
│   └── Persistence/                  # Realization of access to DB
│       ├── Configurations/           # Fluent API
│       ├── Repositories/             # Repositories
│       ├── BankDbContext.cs          # Main context of Entity Framework
│       └── UnitOfWork.cs             # Pattern for transactions
│
└── MyBank.Tests/                     # Testing
    ├── testsDomain/                  # Unit-tests
    ├── testsService/                 # Unit/Integration tests to check correctness of business logic
    └── TestBase.cs                   # Base class with setup
```
## Example of API usage





