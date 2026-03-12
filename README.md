# 💳 Wallet API

![.NET](https://img.shields.io/badge/.NET-ASP.NET%20Core-blue)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green)
![Design](https://img.shields.io/badge/Design-DDD%20%7C%20SOLID-orange)
![Database](https://img.shields.io/badge/Database-SQL%20Server-lightgrey)
![License](https://img.shields.io/badge/License-MIT-yellow)

A **fintech-style digital wallet** backend built with **ASP.NET Core Web API**, designed to simulate how modern digital wallet platforms handle financial transactions, wallet management, and transaction tracking.

The project demonstrates backend engineering practices including:
- Clean Architecture
- Domain-Driven Design principles
- SOLID design principles
- RESTful API design
- Entity Framework Core data persistence
- Dependency Injection and repository pattern

This project was built to demonstrate backend engineering skills and architectural thinking when designing financial systems.

---

## 📑 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Architecture Diagram](#architecture-diagram)
- [Tech Stack](#tech-stack)
- [Engineering Concepts Demonstrated](#engineering-concepts-demonstrated)
- [Features](#features)
- [API Endpoints](#api-endpoints)
- [Example Requests](#example-requests)
- [Running the Project](#running-the-project)
- [What I Learned](#what-i-learned)
- [Future Improvements](#future-improvements)
- [Why I Built This Project](#why-i-built-this-project)
- [Author](#author)
- [License](#license)

---

## Overview

The Wallet API simulates a digital wallet platform similar to fintech applications used for sending and receiving money.

Users can:
- Create wallets
- Add funds (via bank deposit, debit card etc.)
- Send funds (to other wallets, bank account, paypal etc.)
- Track transaction history
- View wallet balances
- Convert from one currency to another

The system focuses on correct financial state management and transactional integrity, ensuring wallet balances and transaction histories remain consistent.

---

## Architecture

This project follows Clean Architecture, separating the system into independent layers.
Each layer has a clearly defined responsibility and communicates through interfaces and abstractions.

### Project Structure
```
Wallet Solution
│
├── Wallet.API
│   └── Controllers
│
├── Wallet.Application
│   ├── DTOs
│   ├── Interfaces
│   └── Services
│
├── Wallet.Domain
│   ├── Entities
│   ├── Enums
│   └── Exceptions
│
└── Wallet.Infrastructure
    ├── Data
    ├── Migrations
    └── Repositories
Layer Responsibilities
```
Domain: 
> Contains core business entities and domain rules.`
> This layer is independent from frameworks and infrastructure.

Application: 
> Implements use cases and coordinates domain logic through services and interfaces.

Infrastructure: 
> Handles database persistence, EF Core configurations, and repository implementations.

API:
> Exposes REST endpoints and configures dependency injection and middleware.

### Architecture Flow
```
Client
   │
   ▼
ASP.NET Core API (Controllers)
   │
   ▼
Application Layer (Use Cases / Services)
   │
   ▼
Domain Layer (Business Rules / Entities)
   │
   ▼
Infrastructure Layer (EF Core / Repositories)
   │
   ▼
SQL Server Database
```
This design ensures the core business logic remains independent of frameworks and external services.

---

## Tech Stack

- ASP.NET Core:	Web API framework
- Entity Framework Core:	ORM for database access
- SQL Server: Data persistence
- Swagger / OpenAPI: API documentation
- Dependency Injection: Service management
- Repository Pattern: Data abstraction
- DTO Pattern: Data transfer between layers
- Result Pattern: Structured error handling

---

## Features

### Wallet Management
- Create wallet
- Retrieve wallet details
- Retrieve all wallets

### Transactions
- Add money
- Send money
- Record transaction history
- Wallet Balance
- Retrieve current wallet balance
```
GET /api/wallets/{walletId}/balance
```
Example response:
```
{
  "walletId": "123",
  "balance": 1150
}
```

---

## API Endpoints

Wallet Endpoints
```
GET    /api/wallets
GET    /api/wallets/{id}
POST   /api/wallets
GET    /api/wallets/{id}/balance
```
Transaction Endpoints
```
POST   /api/wallets/deposit
POST   /api/wallets/transfer
GET    /api/wallets/{walletId}/transactions
POST   /api/transactions/{transactionId}/reverse
```

## ▶ Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio or VS Code

### Clone Repository
```
git clone https://github.com/Jacob-Dev-Lab/fintech-wallet-api.git
```
### Navigate to Project
```
cd fintech-wallet-api
```
### Configure Database
Update connection string in:
```
appsettings.json
```
Example:
```
"ConnectionStrings": {
  "DefaultConnection": "your-database-connection-string"
}
```
### Apply Database Migrations
```
dotnet ef database update
```
### Run the Application
```
dotnet run
```
### API Documentation
Swagger is enabled for interactive API testing.
After running the application open:
```
https://localhost:{port}/swagger
```
Swagger provides a UI to test endpoints directly from the browser.

---

## What I Learned

Building this project provided practical experience in:
- Designing scalable backend architectures
- Applying Clean Architecture
- Implementing Domain-Driven Design principles
- Structuring maintainable backend systems
- Building RESTful APIs with ASP.NET Core
- Integrating SQL Server with Entity Framework Core
- Implementing repository pattern and dependency injection
- Modeling financial transaction systems

---

## Future Improvements

Planned improvements include:
- JWT Authentication and authorization
- Funding channels abstraction (Debit Card / Bank Transfer)
- Currency conversion support
- Database transactions for atomic transfers
- Optimistic concurrency control
- Rate limiting for API protection
- Unit testing with xUnit
- Integration testing
- Docker containerization
- Logging and monitoring
- CI/CD pipeline

---

## Why I Built This Project

This project was built to demonstrate the ability to:
1. Design maintainable backend architectures
2. Apply real-world software engineering principles
3. Model financial domain logic
4. Implement scalable REST APIs using ASP.NET Core

It also serves as a learning platform for exploring how financial systems manage transactions and maintain data consistency.

---
## Author

Backend developer focused on building scalable backend systems and improving software architecture.

### 🔗 Connect
YouTube:  
`https://www.youtube.com/@dotnetdevjourneywithjacob` [(youtube.com in Bing)](https://www.bing.com/search?q="https%3A%2F%2Fwww.youtube.com%2F%40dotnetdevjourneywithjacob")

LinkedIn:  
`https://www.linkedin.com/in/jacoboluwajuwon` [(linkedin.com in Bing)](https://www.bing.com/search?q="https%3A%2F%2Fwww.linkedin.com%2Fin%2Fjacoboluwajuwon")

---

## License

This project is licensed under the MIT License.