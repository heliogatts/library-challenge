# Library Management System

This is a full-stack Library Management System built with .NET 10 Minimal APIs and Angular.

## Architecture

> 📖 For visual diagrams and detailed architectural decision records (ADRs), see [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

### Backend (.NET 10)
- **Minimal APIs** organized using **Vertical Slice Architecture** (Feature folders).
- **REPR Pattern** (Request-Endpoint-Response) for clear input/output definitions.
- **FluentValidation** integrated via endpoint filters.
- **Global Error Handling** returning RFC 7807 `ProblemDetails`.
- **Entity Framework Core** with **PostgreSQL** (using projections to prevent N+1 queries).
- **Interactive API Documentation** via **OpenAPI 3.1 & Scalar** (`/scalar/v1`).
- **Health Checks** endpoint at `/health`.
- **Serilog** for structured JSON logging.

### Frontend (Angular)
- **Standalone Components** and **Signals** (`signal()`, `effect()`, `input()`, `output()`).
- **Modern Responsive Design** with custom properties and design tokens.
- **Services** with HTTP clients and strongly-typed models.
- Features are logically separated mimicking the backend slices (Books, Authors, Genres).

### Containerization & Infrastructure
- Docker and Docker Compose orchestrate the application.
- Uses `nginx` to reverse-proxy frontend requests to `/api/`, `/scalar/`, and `/health` directly to the backend.

## Getting Started

### Using Docker Compose
1. Initialize local secrets (one-time setup):
   ```bash
   ./scripts/init-secrets.sh
   ```
2. Start the services:
   ```bash
   docker compose up --build
   ```
3. The application will be accessible at:
   - Frontend: [http://localhost](http://localhost)
   - Interactive API Docs (Scalar): [http://localhost/scalar/v1](http://localhost/scalar/v1)
   - API Health Check: [http://localhost/health](http://localhost/health)
   - Database: `localhost:5432`

### Default Credentials & Access

| Target | Access / Credentials | Details |
| :--- | :--- | :--- |
| **Web UI** | **No login required** | The frontend at [http://localhost](http://localhost) is open out-of-the-box with initial seeded catalog data (6 books, 4 authors, 3 genres). |
| **Interactive API Docs** | **No authentication required** | Explore and execute endpoints directly at [http://localhost/scalar/v1](http://localhost/scalar/v1). |
| **PostgreSQL Database** | **Host:** `localhost` / `127.0.0.1`<br>**Port:** `5432`<br>**Database:** `librarydb`<br>**User:** `library`<br>**Password:** See below | To connect via pgAdmin, DBeaver, or psql:<br>• **Docker Compose:** Read from `secrets/db_password.txt` (`cat secrets/db_password.txt`)<br>• **Bare-Metal:** `library_secret` |


### Running Bare-Metal (Locally)
If you do not wish to run the app using Docker Compose:

1. **Start PostgreSQL Database**:
   ```bash
   docker run --name library-db -e POSTGRES_USER=library -e POSTGRES_PASSWORD=library_secret -e POSTGRES_DB=librarydb -p 5432:5432 -d postgres:17-alpine
   ```

2. **Start the Backend API**:
   ```bash
   cd src/LibraryApi
   dotnet build
   dotnet run --environment Development
   ```
   *The API will be available at http://localhost:5000*

3. **Start the Angular Frontend**:
   ```bash
   cd src/library-web
   npm install
   ng serve --port 4200
   ```
   *The Web app will be available at http://localhost:4200*

## Testing
Integration tests are provided using `WebApplicationFactory` and `Testcontainers.PostgreSql`.

Run tests via:
```bash
cd tests/LibraryApi.IntegrationTests
dotnet test
```

## Trade-offs and Limitations
- **Security/Auth**: JWT Authentication and Role-based access control (OAuth/OIDC) was skipped to adhere to the scope limits.
- **Testing**: Only backend integration tests were prioritized in the timeframe. Frontend tests are scaffolded but not fully covered.
- **Soft Deletes**: Not implemented. Hard deletes are used, with restrictive foreign keys preventing deletion of entities with children (e.g., Genres with Books).
- **Pagination**: Implemented using offset-based pagination (`Skip`/`Take`). For larger datasets, keyset pagination would be more performant.
