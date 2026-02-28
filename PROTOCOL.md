# Treasure Bay (MRP) - Development Protocol
**Stundent Name:** Vincent Leonardo Müller<br>
**Personal Identifier:** if24b197
## 1. Description of Technical Steps & Architecture Decisions
This document outlines the architectural decisions and technical implementation details for the intermediate submission of the Media Ratings Platform (MRP) project,<br> here codenamed "Pirate Bay" after my original intention of creating a Pirate themed service (Not that you can notice it now).
### 1.1 Core Technology Stack
- **Language:** C# with .NET 9.
- **Server Implementation:** As per the project specification, a low-level (local) HTTP server was required. The `System.Net.HttpListener` class was chosen as it provides the necessary functionality<br> to listen for and respond to raw HTTP requests without relying on a high-level web framework like ASP.NET.
- **Additional Packages:** The `Newtonsoft.Json` (Json.NET) and `BCrypt.Net-Next` (BCrypt.NET) libraries were integrated via NuGet. The former for serializing JSON strings (for responses) and deserializing JSON strings to C# Objects (for requests).<br> The latter was exclusively to utilise a low-grade password hashing method (I didn't know which one to take this one showed up first :>).
### 1.2 Server Architecture
The server is designed to be asynchronous and multi-threaded to handle multiple concurrent client requests efficiently.
- **Asynchronous Request Handling:** The main server loop uses `async/await` with `HttpListener.GetContextAsync()`. This prevents the main thread from blocking while waiting for a request.
- **Concurrent Processing:** Upon receiving a request, the context is immediately dispatched to a new background thread using `Task.Run()`.<br> This "fire-and-forget" approach allows the main listener loop to instantly go back to waiting for new requests.
- **Three-Tier Separation:** The application logic is strictly divided into three layers to enforce the Single Responsibility Principle:
    1.  **Controllers:** Handle incoming HTTP requests, extract parameters/body data, and return formatted HTTP responses.
    2.  **Services:** Contain the core business logic, validation, and authorisation checks.
    3.  **Repositories:** Handle all direct database interactions and raw SQL queries.
### 1.3 Applied SOLID Principles
To ensure a maintainable and scalable codebase, the application architecture strictly adheres to core object-oriented design principles (SOLID):
**1. Single Responsibility Principle (SRP)**
A class should have one, and only one, reason to change. 
* *Real Example:* Initially, the routing logic, HTTP extraction, and database persistence were all combined. This was refactored into a three-tier architecture. For example, the `UserController` is now *only* responsible for parsing incoming HTTP requests and formatting JSON responses. The actual business logic of rules such as the max or minimum amount of poeple displayed on the Leaderboard or validating users is entirely delegated to the `UserService`. If the way we wish to do this changes, the `UserController` remains untouched, proving the responsibilities are safely isolated.

**2. Dependency Inversion Principle (DIP)**
High-level modules should not depend on low-level modules; both should depend on abstractions. Furthermore, classes should not create their own dependencies.
* *Real Example:* Instead of the `RatingController` creating a new instance of the `RatingService` inside its own code using the `new` keyword (which tightly couples them together), the service is injected into the controller via its constructor (`public RatingController(RatingService ratingService...)`). This means the controller simply receives the tool it needs from the main program at startup. This made writing the `RatingServiceTests` exceptionally easy, as I could inject a `FakeRatingRepository` into the service during testing without having to rewrite any core application logic.

**3. Open/Closed Principle (OCP)**
Software entities should be open for extension, but closed for modification.
* *Real Example:* The `BaseController` contains the core logic for checking bearer tokens (`Authenticate`) and returning HTTP data (`SendResponseAsync`). If I want to add an entirely new feature to the API—like a `CommentController`—I simply extend `BaseController`. I am extending the application's capabilities (open for extension) without ever needing to touch or modify the core authentication logic inside `BaseController` (closed for modification).

**4. Interface Segregation Principle (ISP)**
No client should be forced to depend on methods it does not use.
* *Real Example:* In the data access layer, the `MediaRepository` implements the `IMediaRepository` interface. The `MediaService` relies strictly on this interface rather than the concrete repository class. This ensures the service layer is only exposed to the specific data-fetching contracts it actually needs to function, keeping the system decoupled and preventing bloated class dependencies (It also has the benefit for being easily mockable for Testing).
### 1.4 REST API & Routing Design
The API adheres to RESTful principles.
- **Routing:** The routing mechanism was refactored from a monolithic switch statement into a dictionary-based routing system (Found in `HttpServer.cs`). This maps specific URL paths directly to their respective controller handlers.
- **Controller Pattern:** The application logic is separated into controller classes (e.g., `UserController`, `MediaController`). Each controller is responsible for a specific domain of the application, adhering to the Single Responsibility Principle (SRP).
- **Data Transfer Objects (DTOs):** DTOs (e.g., `MediaResponseDTO`, `UserResponseDTO`) are used to shape the data leaving the API. This decouples the database schema from the frontend contract and ensures sensitive data (like password hashes) is stripped before JSON serialization.
- **Inheritance for Code Reuse:** A `BaseController` class was created to hold common logic shared by all controllers. This includes the `SendResponseAsync` helper method for sending HTTP responses and the `Authenticate` method for handling token validation.<br> This avoids code duplication and centralises core functionality.
### 1.5 Data Persistence
While the intermediate submission used in-memory static collections, the final application persists all data using a PostgreSQL database.
- **Relational Mapping:** The database uses foreign keys to link media to creators, ratings to media and users, and track user favourites.
- **SQL Operations:** The `NpgsqlConnection` and `NpgsqlCommand` classes execute raw SQL statements. Complex aggregations, such as calculating the average rating of a media item, are offloaded directly to the database engine using SQL `COALESCE` and subqueries to improve C# execution speed (and avoid N+1 problems).
### 1.6 Security
- **SQL Injection Prevention:** All database queries utilise parameterised inputs (`cmd.Parameters.AddWithValue`) to completely eliminate the risk of SQL injection attacks.
- **Password Hashing:** User passwords are securely hashed using a BCrypt algorithm upon registration. Plain text passwords are never stored.
- **Token-Based Authentication:** After a successful login, a bearer token is generated. Protected endpoints require this token in the `Authorization: Bearer <token>` header, which the `Authenticate` method validates. This Token is kept simple as complex encryption was deemed outside the scope of this project (I forgot how BCrypt works and was too lazy to learn it again to generate super-safe tokens).
## 2 Problems Encountered & Solutions
1.  **Problem:** Initial design placed all logic in `Program.cs`, leading to a large and unmanageable architecture.
    - **Solution:** Refactored the architecture to use the Controller/Service/Repository pattern, improving code organisation and adhering to the Single Responsibility Principle.
2.  **Problem:** Accidental data leaks through JSON serialization, specifically outputting user password hashes when returning raw database models.
    - **Solution:** Implemented Data Transfer Objects (DTOs). By mapping the raw domain models to DTOs before serialization, explicit control over the output payload was established, safely hiding sensitive properties.
3.  **Problem:** Partial updates (PUT requests) were wiping existing database data (like release years) when the client omitted those fields from the JSON body.
    - **Solution:** Updated the request helper classes to use nullable types (`int?`). Combined this with the null-coalescing operator (`??`) in the controller logic to safely fall back to the existing database values if the incoming data was null.
4.  **Problem:** Database conflict errors were being silently ignored during "Add Favourite" operations due to an `ON CONFLICT DO NOTHING` SQL clause, resulting in incorrect HTTP 201 responses instead of 409 Conflicts.
    - **Solution:** Removed the silent conflict clause from the SQL query and implemented a `try/catch` block for `PostgresException` in the C# layer. This correctly maps the `23505` (Unique Violation) SQL state to an HTTP 409 response.
## 3 Estimated Time Tracking
*(Note: Times have been updated to reflect the full development lifecycle, including database integration, testing, and multiple architectural refactors but are estimated as a detailed time-tracking was not kept. Time pertaining to Research / Learning not included).*
| Task Description | Estimated Time (Hours) |
| :--- | :--- |
| Initial Project Setup & Class Design | 3.5 |
| `HttpListener` Server Implementation | 1.5 |
| API Routing Logic & Dictionary Mapping | 1.5 |
| Controller Pattern Implementation | 2.0 |
| User Registration, BCrypt & Token Auth | 3.5 |
| PostgreSQL Database Setup & Schema Design | 3.5 |
| Repository Layer Implementation (SQL CRUD) | 5.5 |
| Service Layer & Business Logic Rules | 3.0 |
| Data Transfer Objects (DTO) Integration | 0.5 |
| Search, Filtering & Advanced SQL Queries | 2.5 |
| Postman API Integration Testing | 3.5 |
| NUnit Unit Testing & Bug Fixing | 3.0 |
| Misceallaneous Refactoring | 2.5 |
| Final Documentation (Protocol & README) | 2.0 |
| **Total** | **38.0** |
## 4. Unit Testing Strategy & Coverage

### 4.1 Testing Strategy
The unit testing strategy is designed to isolate the core business logic from external dependencies, such as the PostgreSQL database and the HTTP network layer. To achieve this, the tests strictly target the **Service Layer** (e.g. `RatingService`). 

Because the architecture utilises Dependency Injection (part of the SOLID principles), I was able to inject a `FakeRatingRepository` into the services during testing. This fake repository simulates database operations in memory. This strategy ensures the unit tests run rapidly and deterministically without requiring a live database connection or risking corruption of actual data. The tests were built and executed using the **NUnit** framework.

### 4.2 Test Coverage
The testing coverage is focused on verifying strict business rules, boundary conditions, and exception handling within the application domain. Key coverage areas include:
* **Valid State Mutations:** Ensuring that valid operations (like creating a rating) successfully update the simulated database and return the correct objects, and that mathematical operations (like sorting `GetTopRatedMedia` by average rating) return lists in the correct order.
* **Boundary Constraints:** Verifying that out-of-bounds inputs are immediately caught by the service layer and throw the expected exceptions.
* **Security & Authorization:** Explicitly testing for `UnauthorizedAccessException` to guarantee that users cannot modify or delete media and ratings that belong to other users.
* **Data Integrity (Not Found):** Confirming that attempting to interact with non-existent data (like deleting a rating ID that does not exist) safely throws a `KeyNotFoundException` rather than crashing the application.

*(Note: While these NUnit tests cover the internal C# business logic, the outer API routing and actual PostgreSQL database interactions were covered via an extensive suite of Postman Integration Tests. Testing Coverage through Unit Testing is limited due to insufficient motiviation on part of the creator).*
## 5 Link to Git Repository
https://github.com/VLM-GNESREK/swen1-semester-proj.git