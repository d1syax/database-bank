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

## Example of API usage




