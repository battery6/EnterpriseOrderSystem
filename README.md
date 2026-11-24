# EnterpriseOrderSystem

EnterpriseOrderSystem is a portfolio backend for demonstrating a complete C#/.NET
delivery workflow: a Clean Architecture API, PostgreSQL persistence, EF Core
migrations, Docker containerization, automated tests, and GitHub Actions CI.

The system is currently an order-management proof of concept. It is designed to
grow into a multi-tenant system with authenticated users, role-based access,
optimistic concurrency, caching, and asynchronous messaging.

## Demonstrated technologies

- C# and .NET 11
- ASP.NET Core Web API
- Clean Architecture
- Entity Framework Core
- PostgreSQL
- FluentValidation
- Docker and Docker Compose
- xUnit
- Testcontainers
- GitHub Actions CI

## Current architecture

```text
HTTP requests
    -> ASP.NET Core controllers
    -> Application commands, queries, handlers, and validators
    -> Domain entities
    -> Repositories and EF Core
    -> PostgreSQL
```

The solution separates the application into:

- `OrderSystem.Domain`: entities and domain rules.
- `OrderSystem.Application`: commands, queries, handlers, and repository contracts.
- `OrderSystem.Infrastructure`: EF Core context, mappings, repositories, migrations.
- `OrderSystem.Api`: controllers, request/response contracts, and middleware.
- `OrderSystem.DbMigrator`: one-shot database migration console application.
- `OrderSystem.UnitTests`: domain, application, repository, and DI tests.
- `OrderSystem.IntegrationTests`: real-PostgreSQL HTTP/API integration tests.

The database model is documented in
[enterpriseOrderDb.drawio](enterpriseOrderDb.drawio).

## Currently implemented

### Domain and application

- Customer, product, order, and order-item entities.
- Domain validation for names, emails, prices, and quantities.
- Create and get operations for customers, products, and orders.
- FluentValidation for application commands.
- Repository interfaces and Unit of Work abstraction.

### API

- REST endpoints for customers, products, and orders.
- JSON request and response contracts.
- Problem Details responses.
- Centralized exception handling.
- Database-backed `/health` endpoint.
- OpenAPI generation in Development mode.

### Persistence and migrations

- EF Core with Npgsql/PostgreSQL.
- EF Core migration history.
- Dedicated `OrderSystem.DbMigrator` console application.
- Migration execution before API startup in Docker Compose.
- PostgreSQL persistence through a named Docker volume.

### Docker

- Multi-stage Dockerfile for the API.
- Separate multi-stage Dockerfile for migrations.
- PostgreSQL and API health/dependency ordering.
- Local environment values through `.env`.
- Docker image builds for development and CI.

### Testing

- Unit tests for domain, application, repositories, and dependency injection.
- PostgreSQL Testcontainers integration tests.
- Database migration and reset in the integration fixture.
- Docker Compose smoke testing.
- Customer, product, and order create/get smoke checks.
- GitHub Actions CI on pushes and pull requests.

## Local setup

### Requirements

- Git
- .NET 11 SDK
- Docker Desktop with Docker Compose
- Git Bash or another Bash-compatible shell

### Configure Docker

Copy the safe template:

```bash
cp .env.example .env
```

Set the database values in `.env`:

```text
POSTGRES_DB=ordersystem
POSTGRES_USER=ordersystem
POSTGRES_PASSWORD=replace-with-a-local-development-password
```

`.env` is local and ignored by Git. `.env.example` is the committed template.

### Run the system

```bash
docker compose up --build -d
```

The stack starts PostgreSQL, waits for it, runs the migrator, and then starts
the API. The API is available at `http://localhost:8080`.

Useful checks:

```bash
docker compose ps
docker compose logs --tail=100 db migrate api
curl -i http://localhost:8080/health
```

Stop containers without deleting database data:

```bash
docker compose down
```

Only use the following when intentionally removing disposable database data:

```bash
docker compose down -v
```

## Running tests

Run all .NET tests:

```bash
dotnet test EnterpriseOrderSystem.sln
```

Run the Docker Compose smoke test:

```bash
bash tests/scripts/docker-smoke.sh
```

Run both local test layers:

```bash
bash tests/scripts/run-all-tests.sh
```

GitHub Actions runs build, unit, integration, and Docker smoke tests on pushes
and pull requests.

## Roadmap

The roadmap is ordered to preserve a stable, demonstrable project at every
stage.

### Phase 0: Project foundation and delivery pipeline

- [x] Create Clean Architecture solution structure.
- [x] Add Domain, Application, Infrastructure, API, and migration projects.
- [x] Add customer, product, order, and order-item domain entities.
- [x] Add basic create and get operations.
- [x] Add FluentValidation command validation.
- [x] Add repository interfaces and implementations.
- [x] Add Unit of Work and dependency injection registration.
- [x] Add PostgreSQL persistence with EF Core and Npgsql.
- [x] Add and apply the initial EF Core migration.
- [x] Add centralized API exception and Problem Details handling.
- [x] Add database-backed `/health` readiness checks.
- [x] Create a dedicated DbMigrator console application.
- [x] Create multi-stage Dockerfiles for the API and migrator.
- [x] Create Docker Compose PostgreSQL, migrator, and API services.
- [x] Add database and migration startup ordering.
- [x] Add persisted PostgreSQL Docker storage.
- [x] Add `.env.example` and ignore local `.env` secrets.
- [x] Add unit tests for domain, application, repositories, and DI.
- [x] Add PostgreSQL Testcontainers integration tests.
- [x] Add Docker Compose smoke tests for migration and core resources.
- [x] Verify customer, product, and order create/get operations.
- [x] Add GitHub Actions CI for restore, build, and test execution.
- [x] Make the CI workflow run integration and Docker smoke tests.

### Phase 1: Complete CRUD and API contracts

- [ ] Add update customer operations.
- [ ] Add delete customer operations.
- [ ] Add update product operations and product-name validation.
- [ ] Add delete product operations with referential-integrity rules.
- [ ] Add update order operations.
- [ ] Add cancel/delete order operations.
- [ ] Add update and remove order-item operations.
- [ ] Add list and paging endpoints.
- [ ] Add filtering and sorting.
- [ ] Return consistent Problem Details for all failures.
- [ ] Test all happy paths and validation/error paths.

Delete operations should use a documented strategy:

- Reject deletion when the entity is still referenced.
- Archive or cancel records where domain history must remain.
- Return a conflict instead of silently deleting dependent data.

### Phase 2: Multi-tenancy and tenant isolation

The existing [database diagram](enterpriseOrderDb.drawio) adds:

- `Tenant`
- `User`
- `TenantMembership`
- Tenant foreign keys on customers, products, orders, and order items.
- Composite uniqueness for tenant-scoped entities.

Implementation steps:

- [ ] Create `Tenant`, `User`, and `TenantMembership` entities.
- [ ] Add tenant identifiers to tenant-owned aggregates.
- [ ] Add EF Core tenant global query filters.
- [ ] Resolve the current tenant from authenticated identity and request context.
- [ ] Add a tenant context service.
- [ ] Enforce tenant IDs when creating and updating records.
- [ ] Add tenant-aware repository and Unit of Work behavior.
- [ ] Generate a migration for all tenant relationships and constraints.
- [ ] Add tenant-isolation integration tests.
- [ ] Add tests proving tenants cannot read or modify one another's records.

Tenant isolation is an application and data-model requirement. It must not rely
only on controllers filtering returned data.

### Phase 3: Authentication and authorization

- [ ] Add ASP.NET Core authentication.
- [ ] Add JWT bearer validation.
- [ ] Add login/user-identity integration.
- [ ] Add tenant membership lookup.
- [ ] Add roles such as tenant administrator and tenant member.
- [ ] Add policy-based authorization.
- [ ] Protect mutating endpoints.
- [ ] Limit tenant administration operations.
- [ ] Add unauthorized and forbidden API tests.
- [ ] Use short-lived access tokens and secure token configuration.

Do not add JWT validation without first defining token issuance, claims, tenant
membership, roles, and refresh-token policy.

### Phase 4: Concurrency, consistency, and domain behavior

- [ ] Add row-version or concurrency-token fields.
- [ ] Convert concurrent-write failures into conflict responses.
- [ ] Prevent duplicate product IDs within an order.
- [ ] Preserve immutable order-item unit prices.
- [ ] Add order status transitions.
- [ ] Add cancellation and refund rules.
- [ ] Add a real stock/reservation model if inventory is demonstrated.
- [ ] Add database transaction boundaries where multiple aggregates change.
- [ ] Decide and test optimistic-concurrency behavior for updates.

### Phase 5: Caching

- [ ] Add Redis to Docker Compose.
- [ ] Cache read-heavy customer, product, and order queries.
- [ ] Include tenant ID in every cache key.
- [ ] Add cache invalidation for mutations.
- [ ] Define cache TTL and versioning.
- [ ] Add cache-hit, cache-miss, and tenant-isolation tests.
- [ ] Measure behavior so Redis demonstrably improves a workload.

Redis should not hide tenant or authorization bugs. It is an optimization after
the database and application behavior are correct.

### Phase 6: Asynchronous messaging

- [ ] Add RabbitMQ to Docker Compose.
- [ ] Add MassTransit producer and consumer setup.
- [ ] Publish domain events after successful writes.
- [ ] Introduce an inbox/outbox pattern if reliability is demonstrated.
- [ ] Add a background audit or notification consumer.
- [ ] Add idempotent consumer handling.
- [ ] Add retry and dead-letter documentation.
- [ ] Test event publication and consumption.

Possible events:

- `CustomerCreated`
- `ProductPriceChanged`
- `OrderCreated`
- `OrderCancelled`
- `TenantProvisioned`

### Phase 7: Observability

- [ ] Add structured Serilog logging.
- [ ] Correlate requests with trace IDs.
- [ ] Add request/response and database timing metrics.
- [ ] Add OpenTelemetry traces and metrics.
- [ ] Add environment and release metadata.
- [ ] Define sensitive log fields that must be redacted.
- [ ] Add health endpoints for liveness and readiness.

The existing `/health` endpoint is an initial PostgreSQL readiness check.
Production-style observability should also distinguish liveness from dependency
readiness.

### Phase 8: Testing maturity

- [ ] Add update/delete API tests.
- [ ] Add tenant-isolation tests.
- [ ] Add authentication and authorization tests.
- [ ] Add concurrency-conflict tests.
- [ ] Add migration upgrade tests.
- [ ] Add a multi-tenant Docker smoke test.
- [ ] Generate test and coverage reports in CI.
- [ ] Add code-quality and formatting checks.
- [ ] Add mutation testing only after core coverage stabilizes.
- [ ] Add load testing for representative order operations.

Keep unit tests fast and database integration tests isolated. Smoke tests should
remain small and workflow-focused.

### Phase 9: CI/CD maturity

Current CI:

- Restore and build.
- Run unit tests.
- Run integration tests.
- Build and run Docker Compose smoke tests.

Next delivery steps:

- [ ] Upgrade action versions where needed.
- [ ] Pin SDK and runtime versions through `global.json`.
- [ ] Publish API and migrator images.
- [ ] Publish images to GitHub Container Registry.
- [ ] Use immutable image tags and commit SHAs.
- [ ] Scan dependencies and container images.
- [ ] Run smoke tests against published staging images.
- [ ] Add environment-specific deployment configuration.
- [ ] Add deployment approval and rollback.
- [ ] Publish test reports and coverage summaries.

This is the transition from CI to Continuous Delivery:

```text
Build and test
    -> scan
    -> version and publish images
    -> deploy to staging
    -> run smoke tests
    -> approve
    -> deploy to production
```

## API development sequence

A practical implementation sequence is:

1. Finish customer CRUD.
2. Finish product CRUD.
3. Finish order and order-item operations.
4. Add pagination and filtering.
5. Add Tenant and TenantMembership.
6. Add tenant-scoped EF query filters.
7. Add authentication and authorization.
8. Add concurrency tokens and conflict handling.
9. Add Redis.
10. Add RabbitMQ/MassTransit.
11. Add observability and deployment automation.

This sequence keeps data ownership, validation, and isolation correct before
adding infrastructure optimizations.

## Portfolio scope

This project should demonstrate the following areas:

- C# and .NET application design.
- Clean Architecture and domain modeling.
- PostgreSQL and EF Core migrations.
- REST API design and validation.
- Docker and Docker Compose.
- Unit and database integration testing.
- Testcontainers.
- CI automation.
- Docker migration and health-check orchestration.
- A clear path toward multi-tenancy and CD.

It should be presented honestly: the current implementation is a tested
order-management proof of concept. Redis, RabbitMQ, JWT, full RBAC,
concurrency control, and deployed CD are roadmap work rather than completed
claims.
