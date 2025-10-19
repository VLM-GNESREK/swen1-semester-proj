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
### 1.3 REST API & Routing Design
The API adheres to RESTful principles.
- **Routing:** A main router is implemented in `Program.cs`. It inspects the `AbsolutePath` of the incoming request URL. Based on the path prefix (e.g., `/api/users`, `/api/media`),<br> it forwards the entire request context to the appropriate controller. This keeps the main program clean and acts as a simple "switchboard".
- **Controller Pattern:** The application logic is separated into controller classes (e.g., `UserController`, `MediaController`). Each controller is responsible for a specific domain of the application, adhering to the Single Responsibility Principle (SRP).<br>**I definitely did not forget about SOLID until the last day and then refactor everything like this on Sunday. Nu uh.**
- **Inheritance for Code Reuse:** A `BaseController` class was created to hold common logic shared by all controllers. This includes the `SendResponseAsync` helper method for sending HTTP responses and the `Authenticate` method for handling token validation.<br> This avoids code duplication and centralises core functionality.
### 1.4 Data Persistence
For the intermediate submission, data is persisted in-memory using `static` collections within the relevant controllers:
- `UserController`: Contains `private static List<User> userDatabase`. 
- `BaseController`: Contains `protected static Dictionary<string, User> tokenDatabase` (Declared `protected` because children need it).
- `MediaController`: Contains `private static List<MediaEntry> mediaDatabase`.
The `static` keyword ensures that there is only one instance of each collection, shared across all requests. This simulates a database for the current runtime session.<br> This will be replaced by a PostgreSQL database in the final version (as far as I saw in Moodle at least this is the plan).
### 1.5 Security
- **Password Hashing:** User passwords are never stored in plain text. Upon registration, the password is securely hashed using a BCrypt algorithm, provided by the `BCrypt.Net-Next` library (this is the only reason I put it in).
- **Token-Based Authentication:** All endpoints (except for `/register` and `/login`) are protected. After a successful login, a unique bearer token is generated and sent to the client.<br> Subsequent requests to protected endpoints must include this token in the `Authorization: Bearer <token>` header. The `Authenticate` method in the `BaseController` validates this token against the `tokenDatabase`.
## 2 Problems Encountered & Solutions
1. **Problem:** Compiler warnings regarding "possible null reference" (`CS8602`) when accessing properties like req.Url.AbsolutePath.
    - **Solution:** Implemented defensive coding by using the null-conditional operator (`?.`) and the null-coalescing operator (`??`) to provide safe access and default values (e.g., `var path = req.Url?.AbsolutePath ?? "/";`).<br> This prevents crashes from malformed requests and satisfies the compiler's null-state analysis.
2. **Problem:** Initial design placed all logic in `Program.cs`, leading to a large and unmanageable `switch` statement (I forgot that SOLID was a thing).
    - **Solution:** Refactored the architecture to use the Controller pattern. Logic was separated into `UserController` and `MediaController`, improving code organisation and adhering to the Single Responsibility Principle.
3. **Problem:** Duplication of code for sending responses and authenticating users in multiple controllers.
    - **Solution:** Created a `BaseController` and used inheritance. Common methods (`SendResponseAsync`, `Authenticate`) were moved to the base class and declared as protected, allowing child controllers to reuse them.
## 3 Estimated Time Tracking
|Task Description|Estimated Time (Hours)|
|----------------|----------------------|
|Initial Project Setup & Class Design|3,5|
|`HttpListener` Server Implementation|1,5|
|API Routing Logic (`Program.cs`)|1,0|
|Refactoring to Controller Pattern|2,0|
|User Registration & Login Endpoints|2,5|
|Password Hashing (`BCrypt`) Integration|0,5|
|Token-Based Authentication Logic|1,5|
|Media Management (CRUD) Endpoints|2,5|
|Postman Testing & Debugging|2,0|
|Documentation (Protocol & README)<br>(I had to make it fancy)|1,5|
|**Total**|18,5|