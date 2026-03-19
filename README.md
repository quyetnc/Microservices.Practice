# ASP.NET Core Microservices Architecture

![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)
![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)
![Microservices](https://img.shields.io/badge/Architecture-Microservices-brightgreen.svg)

A complete, production-ready microservices solution built with **.NET 10**, focusing on **Clean Architecture**, **CQRS**, and **Event-Driven Design**.

## 🏗 Architecture Overview

This project implements a robust microservices architecture demonstrating best practices for distributed systems. The services communicate asynchronously via an Event Bus and store data in polyglot databases.

### Key Microservices:
- **Product API**: Catalog management using **MySQL**.
- **Customer API**: Customer profile management using **PostgreSQL**.
- **Ordering API**: Order processing applying CQRS and Clean Architecture using **SQL Server**.
- **Basket API**: Shopping cart management using **Redis**.
- **Inventory API**: Stock tracking using **MongoDB**.

### Core Infrastructure:
- **Message Broker**: RabbitMQ
- **API Gateway**: Ocelot
- **Logging & Monitoring**: Serilog, Elasticsearch, and Kibana
- **Container Management**: Docker, Docker Compose, Portainer

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop)
- IDE (Visual Studio 2022, Rider, or VS Code)

### Running Locally via Docker
To spin up the entire infrastructure and all microservices, simply use Docker Compose:

```bash
# Build and run containers in detached mode
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build --remove-orphans

# To stop and remove the containers
docker-compose down
```

### Local Development Commands
If you want to run services via the .NET CLI:
```bash
# Restore packages
dotnet restore

# Run a specific service with hot-reload
dotnet watch run --environment "Development"
```

---

## 🔗 Useful URLs & Endpoints

Once the infrastructure is up, you can access the following management dashboards and APIs:

### Infrastructure Control Panels
| Service | URL | Default Credentials |
|---------|-----|---------------------|
| **Portainer** | [http://localhost:9000](http://localhost:9000) | `admin` / `Climax!@#` |
| **Kibana** | [http://localhost:5601](http://localhost:5601) | `elastic` / `admin` |
| **RabbitMQ** | [http://localhost:15672](http://localhost:15672) | `guest` / `guest` |
| **pgAdmin** | [http://localhost:5050](http://localhost:5050) | `admin@tedu.com.vn` / `admin1234` |

### API Endpoints
| API | Route |
|-----|-------|
| **Product API** | `http://localhost:6002/api/products` |
| **Customer API** | `http://localhost:6003/api/customers` |
| **Basket API** | `http://localhost:6004/api/baskets` |
| **Ordering API** | `http://localhost:6005/api/v1/orders` |

*(Ensure these match your actual mapped ports inside `docker-compose.override.yml`)*

---

## 🗄 Entity Framework Core Migrations

This solution uses Code-First migrations across multiple databases. Below are the commands utilized:

### Product DB (MySQL)
Run from the `Product.API` directory:
```bash
dotnet ef migrations add "Init_ProductDB"
dotnet ef database update
```

### Ordering DB (SQL Server)
The Ordering service follows **Clean Architecture**, separating the API and Infrastructure layers. 

Run from the `src/Services/Ordering` directory:
```bash
# Add a new migration
dotnet ef migrations add "Init_OrderDB" --project Ordering.Infrastructure --startup-project Ordering.API --output-dir Persistence/Migrations

# Remove the last migration
dotnet ef migrations remove --project Ordering.Infrastructure --startup-project Ordering.API

# Update database
dotnet ef database update --project Ordering.Infrastructure --startup-project Ordering.API 
```
> **Note**: For production environments, use `ASPNETCORE_ENVIRONMENT=Production dotnet ef database update`.

---
*Developed with ❤️ using .NET 10 and Docker.*