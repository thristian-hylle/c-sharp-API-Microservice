# ProductService

A simple C# microservice built with ASP.NET Core, PostgreSQL, and Kafka.

## What this project does

This service exposes a small Products API.

It allows you to:
- get all products
- get one product by id
- create a new product

When a product is created:
1. the API receives the request
2. the product is saved to PostgreSQL
3. the service can publish a `product.created` event to Kafka

## Tech stack

- C#
- .NET 8 / ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Kafka
- Docker

## Project structure

- `Program.cs` → app startup and API endpoints
- `Product.cs` → product model
- `CreateProductRequest.cs` → request model for creating a product
- `AppDbContext.cs` → database context for PostgreSQL
- `KafkaService.cs` → sends messages to Kafka

## How to run the project

### 1. Start PostgreSQL with Docker

```bash
docker run -d \
  --name postgres-db \
  -e POSTGRES_DB=productsdb \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16
