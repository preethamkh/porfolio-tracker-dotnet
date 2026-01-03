# PortfolioTracker.Core Design Overview

## Purpose
`PortfolioTracker.Core` contains the core business logic, data transfer objects (DTOs), shared abstractions, and domain services for the Portfolio Tracker application. It is designed to be independent of infrastructure concerns, focusing on domain models, validation, and service contracts.

## Folder Structure

- `DTOs\Holding\`  
  Contains DTOs related to holdings, representing assets or positions in a portfolio.

- `DTOs\Common\`  
  Contains shared or generic DTOs used across multiple domains (e.g., error responses, pagination).

- `DTOs\Transaction\`  
  Contains DTOs for transactions, such as buy/sell operations or transfers.

- `Entities\`  
  Contains domain entities representing core business objects (e.g., Portfolio, Asset, User). Entities encapsulate business state and behavior, distinct from DTOs.

- `Interfaces\`  
  Contains service and repository interfaces that define contracts for business logic and data access.

- `Services\`  
  Business logic services. Work with our domain entities.These orchestrate business rules and domain operations

- `Configuration\`  
  Contains configuration abstractions and settings classes used by the core layer. This enables centralized management of options and settings relevant to business logic.

## Dependencies

- `BCrypt.Net-Next`  
  Used for secure password hashing and verification.

- `Microsoft.Extensions.Logging`  
  Provides logging abstractions for use in core services.

- `System.IdentityModel.Tokens.Jwt`  
  Supports JWT token creation and validation for authentication and authorization.

## Design Principles

- **Separation of Concerns:**  
  Core logic, DTOs, entities, interfaces, and services are isolated from infrastructure and presentation layers.

- **Reusability:**  
  DTOs, entities, services, interfaces, and abstractions are designed for use across multiple projects (e.g., Infrastructure, API).

- **Extensibility:**  
  The structure allows for easy addition of new domains (e.g., new DTO folders, entities, interfaces, services).

## Usage

- Reference this project from infrastructure and API layers to access business logic, shared models, domain entities, interfaces, and services.
- Implement domain services, entities, and validation logic here, keeping dependencies minimal and focused on business rules.

## Future Extensions

- Add additional domain services and interfaces for business operations.
- Expand DTOs to cover additional portfolio features (e.g., analytics, reporting).
- Extend entities and configuration abstractions to support new business requirements.
