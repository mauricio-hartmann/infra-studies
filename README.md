# Infra Studies

Infra Studies is a learning project that simulates a ticket management system inspired by platforms such as Zendesk. It provides a practical environment for exploring infrastructure, software architecture, and modern backend technologies while modeling capabilities related to customers and support tickets.

The project is organized as a set of ASP.NET Core Web APIs with shared building blocks and containerized infrastructure. Its focus is experimentation and continuous learning rather than reproducing every feature of a production ticketing platform.

## Technologies and Concepts

- .NET 10 and ASP.NET Core Web API
- PostgreSQL with the `pg_trgm` extension
- Entity Framework Core 10 and code-first migrations
- Redis and the cache-aside pattern
- RabbitMQ for messaging infrastructure
- Docker
- OpenAPI
- Serilog with console and PostgreSQL sinks
- FluentValidation
- xUnit, Moq, AutoFixture, and test coverage with Coverlet
- Vertical Slice Architecture
- Command Query Responsibility Segregation (CQRS)
- Domain-driven design building blocks, including entities and aggregate roots
- Pagination and normalized text search
- Dependency injection and shared infrastructure libraries
- Global exception handling with Problem Details responses
- Liveness and readiness health checks
- Asynchronous programming and cancellation token propagation

## Next steps
- Transactional outbox and inbox pattern
- RabbitMQ implementation
- CDC (Debezium like)
- ELK
- K8S