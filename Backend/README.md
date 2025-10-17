# Backend API - Clean Architecture

## Project Structure

```
Backend/
├── Controllers/              # API endpoints and controllers
├── Models/
│   ├── Entities/            # Domain models (database entities)
│   ├── DTOs/                # Data Transfer Objects (request/response models)
│   └── Responses/           # API response wrappers
├── Services/
│   ├── Interfaces/          # Service contracts/interfaces
│   └── Implementations/     # Service business logic implementations
├── Repositories/
│   ├── Interfaces/          # Repository contracts/interfaces
│   └── Implementations/     # Data access implementations
├── Data/
│   ├── Context/             # DbContext and database configuration
│   └── Configurations/      # Entity configurations (Fluent API)
├── Middleware/              # Custom middleware (error handling, logging, etc.)
├── Extensions/              # Extension methods and service registration
├── Validators/              # Input validation logic
└── Program.cs               # Application entry point
```

## Architecture Layers

### Controllers
- Handle HTTP requests and responses
- Call services to execute business logic
- Return appropriate HTTP status codes

### Models
- **Entities**: Database models with navigation properties
- **DTOs**: Simplified data structures for API communication
- **Responses**: Standardized API response formats

### Services
- Contain business logic
- Orchestrate between repositories and controllers
- Should be stateless

### Repositories
- Handle data access and persistence
- Implement CRUD operations
- Abstract the database layer

### Data
- Entity Framework Core configuration
- Database context and connection management

### Middleware
- Cross-cutting concerns (logging, error handling)
- Request/response processing

## Getting Started

1. Install required packages:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

2. Configure your database connection in `appsettings.json`

3. Create your entities in `Models/Entities/`

4. Create your DbContext in `Data/Context/`

5. Run migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Running the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001` or `http://localhost:5000`
