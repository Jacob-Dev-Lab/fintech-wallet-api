# 💳 Wallet API (Fintech Backend)

![.NET](https://img.shields.io/badge/.NET-ASP.NET%20Core-blue)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green)
![Design](https://img.shields.io/badge/Design-DDD%20%7C%20SOLID-orange)
![Database](https://img.shields.io/badge/Database-SQL%20Server-lightgrey)
![License](https://img.shields.io/badge/License-MIT-yellow)

A **production-style fintech wallet backend** built with **ASP.NET Core Web API**, designed to simulate how modern financial platforms securely manage wallets, transactions, and user authentication.

The project demonstrates backend engineering practices including:
- Clean Architecture
- Domain-Driven Design principles
- SOLID design principles
- RESTful API design
- Entity Framework Core data persistence
- Dependency Injection and repository pattern

This project was built to demonstrate backend engineering skills and architectural thinking when designing financial systems.

---

## 🚀 Overview

This project models a secure digital wallet system where users can:

- Register and authenticate
- Create and manage wallets
- Deposit, withdraw and transfer funds
- View transaction history
- Maintain consistent financial state

The system emphasizes:

- Security (JWT authentication, password hashing)
- Clean Architecture & separation of concerns
- Domain-driven design (DDD)
- Transactional integrity
- Concurrency safety

---

## 🏗️ Architecture

This project follows Clean Architecture, ensuring maintainability and scalability.

### 📂 Project Structure
```
Wallet Solution
│
├── Wallet.API            → Controllers, Middleware, Config
├── Wallet.Application    → Use cases, DTOs, Interfaces, Business Logics
├── Wallet.Domain         → Entities, Enums, Business Rules
└── Wallet.Infrastructure → EF Core, Repositories, Persistence
```

### 🔄 Flow
```
Client → API → Application → Domain → Infrastructure → Database
```

### 🧠 Layer Responsibilities

- **Domain**
  - Core business entities
  - Enforces business rules and invariants
  - Framework-independent

- **Application**
  - Orchestrates use cases
  - Handles business logic
  - Converts domain behavior into results

- **Infrastructure**
  - Handles data persistence (EF Core)
  - Implements repositories

- **API**
  - Handles HTTP requests
  - Performs validation
  - Maps responses

---

## 🧠 Key Engineering Concepts

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID Principles
- Repository Pattern
- Unit of Work Pattern
- Result Pattern (structured error handling)
- Optimistic Concurrency Control
- RESTful API Design

---

## 🛠️ Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- FluentValidation (manual validation)
- Swagger (OpenAPI)
- JWT Authentication

---

## 🔐 Security Features

- JWT-based authentication
- Protected endpoints using [Authorize]
- Password hashing (no plain text storage)
- User identity extracted from claims
- Ownership-based resource access

---

## ⚙️ Advanced Features Implemented

### 🔄 Concurrency Control

To prevent race conditions and ensure data consistency:

- Implemented **optimistic concurrency** using `RowVersion`
- Detects conflicting updates automatically via EF Core
- Returns **409 Conflict** when concurrent modifications occur
- Prevents lost updates in high-concurrency scenarios

### ✅ Validation Strategy

- Implemented input validation using **FluentValidation**
- Used **manual validation in controllers** for learning clarity
- Ensures only valid data reaches the application layer

### ⚠️ Error Handling

- Global exception handling middleware for unexpected errors
- Result pattern for controlled application errors
- Centralized HTTP response mapping
- Consistent API error responses

---

### ⚡ Performance Considerations

- Optimized email validation using a **singleton Regex instance**
- Enhanced repository performance through query optimization
- Reduced repeated allocations and GC pressure
- Improved performance under concurrent requests
- Introduced database indexing to improve query execution and scalability
- API response standardization (ProblemDetails)

---

## Features

### 👤 Authentication
- Register user
- Login user (JWT token generation)

### 💼 Wallet Management
- Create wallet
- Retrieve wallet details
- Freeze/Unfreeze Wallet
- Deposit funds
- Withdraw funds
- Transfer funds between wallets

### 💸 Transactions

- View transaction history

---

## 📡 API Endpoints

### 🔐 Auth
```
POST   /api/account/register
POST   /api/account/login
```

### 💼 Wallet Endpoints
```
GET    /api/wallets
GET    /api/wallets/{walletId}
POST   /api/wallets
POST   /api/wallets/{walletId}/freeze
POST   /api/wallets/{walletId}/unfreeze
POST   /api/wallets/{walletId}/deposit
POST   /api/wallets/{walletId}/withdraw
POST   /api/wallets/{WalletId}/transfer
```

### 💸 Transaction Endpoints
```
GET   /api/Transactions
GET   /api/Transactions/{walletId}
```

## ▶ Getting Started (Local)

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

📚 What I Learned

- Designing scalable backend systems
- Implementing secure authentication systems
- Managing concurrency and data consistency
- Structuring maintainable architectures
- Handling real-world financial transaction logic
- Building production-ready REST APIs
- Optimizing queries for efficient database performance

---

## 🚧 Future Improvements

- Automatic FluentValidation integration (pipeline/filter)
- Logging with Serilog
- Retry policies for concurrency conflicts
- Unit & integration testing
- Rate limiting & API security hardening
- Containerization (Docker)
- CI/CD pipeline
- Cloud deployment (AWS/Azure)

---

## Why I Built This Project

This project demonstrates my ability to:
1. Design scalable backend architectures
2. Apply real-world software engineering principles
3. Model financial domain logic
4. Build production-ready APIs with ASP.NET Core

It also serves as a hands-on exploration of how financial systems ensure data integrity, security, and consistency.

---

## 👨‍💻 Author

Backend developer focused on building scalable and secure systems.

### 🔗 Connect
YouTube:  
`https://www.youtube.com/@dotnetdevjourneywithjacob` [(youtube.com in Bing)](https://www.bing.com/search?q="https%3A%2F%2Fwww.youtube.com%2F%40dotnetdevjourneywithjacob")

LinkedIn:  
`https://www.linkedin.com/in/jacoboluwajuwon` [(linkedin.com in Bing)](https://www.bing.com/search?q="https%3A%2F%2Fwww.linkedin.com%2Fin%2Fjacoboluwajuwon")

---

## 📄 License

This project is licensed under the MIT License.