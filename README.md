# HysEnterpiseTestTask

## Overview

HysEnterpiseTestTask is a modular .NET solution designed to demonstrate clean architecture principles, domain-driven design, and best practices for building scalable enterprise applications. The project is organized into multiple layers, including API, Application, Domain, and Persistence, to ensure separation of concerns and maintainability.

## Features

- **Clean Architecture**: Clear separation between API, Application, Domain, and Persistence layers.
- **CQRS Pattern**: Command and Query Responsibility Segregation for scalable and maintainable code.
- **Dependency Injection**: Easily swap implementations for testing and flexibility.
- **Unit Testing**: Includes a dedicated test project for business logic validation.
- **Extensible Domain Model**: Entities and value objects for meetings, users, and more.

## Project Structure

```
HysEnterpiseTestTask.sln
├── API/                # ASP.NET Core Web API
├── Application/        # Application logic, CQRS handlers
├── Application.Tests/  # Unit tests for Application layer
├── Domain/             # Domain entities, value objects, interfaces
└── Persistence/        # Data access and repository implementations
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- IDE such as [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)

### Build and Run

1. **Clone the repository:**
   ```sh
   git clone https://github.com/mkhrebtan/HysEnterpiseTestTask.git
   cd HysEnterpiseTestTask
   ```
2. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
3. **Build the solution:**
   ```sh
   dotnet build
   ```
4. **Run the API project:**
   ```sh
   dotnet run --project API/API.csproj
   ```

### Running Tests

```sh
dotnet test Application.Tests/Application.Tests.csproj
```

## Usage

The API exposes endpoints for managing meetings and users. You can use tools like [Postman](https://www.postman.com/) to interact with the API.

## License

This project is licensed under the MIT License.