# GitHub Copilot Instructions for 3D Platform Services

## Project Overview
This is a .NET 7.0 microservices-based platform for 3D estate services, implementing Clean Architecture with Domain-Driven Design (DDD) patterns. The solution manages core products, images, interior designers, and tour style configurations.

## Architecture & Structure

### Solution Organization
- **ApiGateways**: API Gateway implementations (Platform, ToursStylesConfigurator)
- **BuildingBlocks**: Shared libraries and cross-cutting concerns
  - Application layer (CQRS, EventBus, Excel, Exceptions, Storage abstractions)
  - Domain layer (DDD primitives)
  - Infrastructure layer (Azure Storage, Cosmos DB, Dapr, Google Sheets, OpenTelemetry)
- **Services**: Microservices implementing business domains
  - CoreProducts
  - ImagesManagement
  - InteriorDesigners
  - ToursStylesConfigurator

### Technology Stack
- **.NET 7.0** (C# 11)
- **Dapr** for service-to-service communication and pub/sub
- **Azure Cosmos DB** for data persistence
- **Azure Blob Storage** for file storage
- **MediatR** for CQRS implementation
- **OpenTelemetry** for observability
- **Docker** for containerization
- **Azure Container Apps** for deployment

## Design Patterns & Principles

### Clean Architecture Layers
Each service follows Clean Architecture with these layers:
1. **Domain**: Aggregates, Entities, Value Objects, Domain Events
2. **Application**: Commands, Queries, DTOs, Integration Events, Services
3. **Infrastructure**: Repositories, External Services, Data Access
4. **Api**: Controllers, Configuration, Middleware

### CQRS Pattern
- Use `ICommand` / `ICommandHandler<TCommand>` for commands (write operations)
- Use `IQuery<TResponse>` / `IQueryHandler<TQuery, TResponse>` for queries (read operations)
- Commands return `Unit` (void) or specific response types
- All handlers are located in the Application layer

**Example Command:**
```csharp
public class AddNewInteriorDesignerCommand : ICommand
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
}

internal class AddNewInteriorDesignerCommandHandler : ICommandHandler<AddNewInteriorDesignerCommand>
{
    public async Task<Unit> Handle(AddNewInteriorDesignerCommand request, CancellationToken cancellationToken)
    {
        // Implementation
        return Unit.Value;
    }
}
```

### Domain-Driven Design (DDD)
- **Aggregates** are root entities that guarantee consistency
- Use **Value Objects** for domain concepts without identity
- Aggregates implement `IDocument` interface for Cosmos DB persistence
- Domain logic belongs in the Domain layer, not in handlers
- Repository interfaces are defined in Application layer, implemented in Infrastructure

**Aggregate Example:**
```csharp
public class InteriorDesignerAggregate : IDocument
{
    public string Code { get; private set; }
    public string DisplayName { get; private set; }
    
    private InteriorDesignerAggregate() { } // For ORM
    
    public static InteriorDesignerAggregate New(string name, string displayName, string externalLink)
    {
        // Factory method with validation
    }
}
```

### Event-Driven Architecture
- Use **Integration Events** for cross-service communication via Dapr pub/sub
- Integration events inherit from `IntegrationEvent` base class
- Mark events with `[TopicName("topic-name")]` attribute
- Event handlers inherit from `IntegrationEventHandler<TEvent>`
- Subscribe to events using `AddSubscription<TEvent>()` in Program.cs

**Integration Event Pattern:**
```csharp
[TopicName("core-products-updated")]
public record CoreProductsUpdatedIntegrationEvent : IntegrationEvent
{
    public List<ProductDto> Products { get; init; }
}

public class CoreProductUpdatedIntegrationEventHandler 
    : IntegrationEventHandler<CoreProductsUpdatedIntegrationEvent>
{
    protected override async Task HandleEvent(CoreProductsUpdatedIntegrationEvent @event, 
        CancellationToken cancellationToken)
    {
        // Handle event
    }
}
```

### Repository Pattern
- Repository interfaces in Application layer
- Implementations in Infrastructure layer using Cosmos DB
- Use identity objects for finding aggregates
- Standard methods: `FindAsync()`, `UpsertAsync()`, `DeleteAsync()`, `GetAllAsync()`

**Repository Pattern:**
```csharp
// Application layer
public interface IInteriorDesignerRepository
{
    Task<InteriorDesignerAggregate?> FindAsync(string id);
    Task UpsertAsync(InteriorDesignerAggregate aggregate, CancellationToken cancellationToken);
}

// Infrastructure layer
public class InteriorDesignerRepository : IInteriorDesignerRepository
{
    private readonly InteriorDesignerContext _context;
    // Implementation
}
```

## Coding Conventions

### Naming Conventions
- **Commands**: Use verb-based names ending with "Command" (e.g., `AddNewInteriorDesignerCommand`)
- **Queries**: Use "Get" prefix and end with "Query" (e.g., `GetInteriorsDesignersQuery`)
- **Handlers**: Append "Handler" to command/query name (e.g., `AddNewInteriorDesignerCommandHandler`)
- **Integration Events**: Describe the event in past tense, end with "IntegrationEvent" (e.g., `CoreProductUpdatedIntegrationEvent`)
- **Repositories**: Interface `I{EntityName}Repository`, implementation `{EntityName}Repository`
- **Aggregates**: End with "Aggregate" (e.g., `InteriorDesignerAggregate`, `ProductAggregate`)

### File Organization
- One class per file
- File name matches the class name
- Organize by feature/use case, not by technical layer
- Group related features in folders (e.g., `/Products`, `/InteriorsDesigners`)

### Dependency Injection
- Register services in `Program.cs` or extension methods
- Use constructor injection exclusively
- Make dependencies explicit via constructor parameters
- Prefer interfaces over concrete implementations

### Async/Await
- All I/O operations must be async
- Use `CancellationToken` parameter in async methods
- Name async methods with "Async" suffix
- Avoid `.Result` or `.Wait()` - always await

### Logging
- Use `ILogger<T>` for logging
- Log at appropriate levels (Information, Warning, Error)
- Include context in log messages
- Use structured logging with named parameters

**Example:**
```csharp
_logger.LogInformation(
    "Processing command {CommandName} for designer {DesignerCode}",
    nameof(AddNewInteriorDesignerCommand),
    request.Code);
```

### Error Handling
- Use custom exceptions from `BuildingBlocks.Application.Exceptions`
- Don't catch exceptions unless you can handle them meaningfully
- Let exceptions bubble up to middleware
- Use domain-specific exceptions for business rule violations

## Project-Specific Patterns

### BuildingBlocks Abstractions
Always use BuildingBlocks for cross-cutting concerns:
- **CQRS**: `BuildingBlocks.Abstractions.CQRS.CQRS`
- **EventBus**: `BuildingBlocks.Application.EventBus`
- **Storage**: `BuildingBlocks.Abstractions.Storage`
- **Excel/Google Sheets**: `BuildingBlocks.Abstractions.Excel`
- **Exceptions**: `BuildingBlocks.Application.Exceptions`

### Cosmos DB Patterns
- Use `IDocument` interface for all persistable entities
- Partition keys are entity-specific (usually Code or Id)
- Use `ToDocumentListAsync()` for query results
- Context classes named `{EntityName}Context`

### Dapr Integration
- Pub/Sub name: `DaprEventBus.PubSubName` (constant)
- Secret store access via Dapr
- Health checks using `BuildingBlocks.Infrastructure.DaprHealthChecks`
- Service-to-service calls via Dapr client

### Configuration
- Use `appsettings.json` and `appsettings.Development.json`
- Sensitive data in Dapr secret store
- Environment-specific settings via environment variables
- Define constants in `ConfigSettings` or `Consts` classes

### Docker & Deployment
- Multi-stage Dockerfiles with SDK and runtime images
- Copy .csproj files first for layer caching
- Use relative paths in COPY statements
- Bicep templates in `/deploy/containerapps`

## Testing Conventions
- Unit tests in `*.Tests` projects
- Integration tests for event handlers
- Mock external dependencies
- Test aggregates in isolation

## API Design
- RESTful endpoints
- Use minimal APIs or controllers
- Version APIs appropriately
- Document with XML comments
- Health check endpoints required

## Common Tasks

### Adding a New Command
1. Create command class implementing `ICommand` or `ICommand<TResponse>`
2. Create handler implementing `ICommandHandler<TCommand>` or `ICommandHandler<TCommand, TResponse>`
3. Add validation if needed (FluentValidation)
4. Register in DI if needed (MediatR auto-registers)
5. Call via `IMediator.Send(command)`

### Adding a New Integration Event
1. Create event record inheriting from `IntegrationEvent`
2. Add `[TopicName("event-name")]` attribute
3. Create handler inheriting from `IntegrationEventHandler<TEvent>`
4. Register subscription in `Program.cs`: `app.AddSubscription<TEvent>()`
5. Publish via `IEventBus.PublishAsync()`

### Adding a New Aggregate
1. Create aggregate class in Domain layer
2. Implement `IDocument` interface
3. Add factory methods (static `New()`)
4. Keep setters private, use methods for mutations
5. Create repository interface in Application layer
6. Implement repository in Infrastructure layer

### Adding a New Microservice
1. Follow existing service structure (Api, Application, Domain, Infrastructure)
2. Add Dockerfile
3. Configure Dapr sidecar
4. Add health checks
5. Register in solution file
6. Add deployment pipeline in `/deploy`

## Performance Considerations
- Use async/await for all I/O
- Implement caching with `IMemoryCache` where appropriate
- Use `CancellationToken` for long-running operations
- Batch operations when possible
- Monitor with OpenTelemetry traces

## Security Best Practices
- Never hardcode secrets
- Use Dapr secret store
- Validate all inputs
- Sanitize user data
- Use authentication/authorization middleware
- Follow least privilege principle

## Code Quality
- Follow C# coding conventions
- Use nullable reference types
- Enable and fix compiler warnings
- Use `records` for DTOs and events
- Prefer immutability
- Keep methods small and focused
- Use descriptive variable names

## Documentation
- Add XML comments to public APIs
- Document complex business logic
- Maintain README files
- Update C4 diagrams when architecture changes
- Document integration points
