# EnterpriseOrderSystem

An enterprise-grade, multi-tenant order management backend built with Clean Architecture, featuring bulletproof optimistic concurrency control, Redis caching, and asynchronous event-driven messaging.

## Stack

- **Core:** C# 15, .NET 11 Web API
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
- **Database & ORM:** PostgreSQL 18.6, Entity Framework Core (EF Core) with Global Query Filters & Optimistic Concurrency
- **Caching & Messaging:** Redis, RabbitMQ, MassTransit
- **Validation & Logging:** FluentValidation, Serilog (Structured Logging)
- **Security:** JWT Bearer Authentication, Role-Based Access Control
- **Testing:** xUnit, FluentAssertions, Testcontainers (Docker-backed integration tests)
- **Infrastructure:** Docker & Docker Compose
