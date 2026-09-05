# Library Management System

A full-stack Library Management System built with **.NET 10 Minimal APIs** and **Angular 22**, designed with clean architecture, container hardening, and production-ready patterns.

> [!TIP]
> ### ⚡ Evaluator Quick Start (Zero Setup)
> Run the complete full-stack environment with a single command:
> ```bash
> docker compose up --build
> ```
> **Access Points & URLs:**
> - 🌐 **Frontend (Angular Web UI)**: [http://localhost](http://localhost) (Docker, port 80) &bull; *Bare-Metal:* [http://localhost:4200](http://localhost:4200)
> - ⚙️ **Backend (.NET Minimal API)**: [http://localhost:5000](http://localhost:5000) (direct) &bull; *Via Reverse Proxy:* [http://localhost/api](http://localhost/api)
> - 📖 **Interactive API Docs (Scalar)**: [http://localhost/scalar/v1](http://localhost/scalar/v1) &bull; *Bare-Metal:* [http://localhost:5000/scalar/v1](http://localhost:5000/scalar/v1)
> - 📜 **OpenAPI 3.1 Specification**: [http://localhost/openapi/v1.json](http://localhost/openapi/v1.json) &bull; *Bare-Metal:* [http://localhost:5000/openapi/v1.json](http://localhost:5000/openapi/v1.json)
> - 💓 **Application Health Check**: [http://localhost/health](http://localhost/health) &bull; *Bare-Metal:* [http://localhost:5000/health](http://localhost:5000/health)
> - 🗄️ **PostgreSQL Database**: `localhost:5432` (db: `librarydb`, user: `library`, password: `library_secret`)
>
> **Run Automated Tests (Unit & Integration):**
> ```bash
> dotnet test
> ```

---

## Architecture

> 📖 For visual diagrams, runtime topologies, and architectural decision records (ADRs), see [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).
>
> *(Note: Diagrams in `ARCHITECTURE.md` are rendered with [Mermaid](https://mermaid.js.org/). If your local markdown previewer doesn't render them, view them on GitHub/GitLab or paste the diagram code into [Mermaid Live Editor](https://mermaid.live/).)*


### Backend (.NET 10)
- **Minimal APIs** organized using **Vertical Slice Architecture** (Feature folders).
- **REPR Pattern** (Request-Endpoint-Response) for clear input/output boundaries.
- **FluentValidation** integrated via endpoint filters with RFC 7807 validation problem details.
- **Centralized Error Handling** via `GlobalExceptionHandler` mapping domain and database constraints to standard HTTP status codes (`409 Conflict`, `422 Unprocessable Entity`).
- **Entity Framework Core 10** with **PostgreSQL 17** (projections to eliminate N+1 queries; `DeleteBehavior.Restrict` to protect relational integrity).
- **Interactive API Documentation** via **OpenAPI 3.1 & Scalar** (`/scalar/v1`).
- **Health Checks** endpoint at `/health`.
- **Serilog** for structured JSON logging.

### Frontend (Angular 22)
- **Standalone Components** and **Signals** (`signal()`, `computed()`, `effect()`, `input()`, `output()`).
- **Modern Responsive UI** styled with pure CSS, custom properties, and design tokens (no heavy UI libraries).
- **Services** with strongly typed HTTP clients (`withFetch()`) and DTO models.
- **Feature Slices** matching the backend (Books, Authors, Genres).
- **Unit Testing** configured with [Vitest](https://vitest.dev/).

### Security & Hardening
- **Docker Secrets**: Database credentials managed via ephemeral secrets (`secrets/db_password.txt` &rarr; `/run/secrets/db_password`). Pre-seeded with development defaults for zero-setup evaluation; optional rotation scripts provided.
- **Distroless Runtime**: Backend runs on Microsoft's Ubuntu Chiseled image (`dotnet/aspnet:10.0-noble-chiseled`) with no package managers or shells.
- **Non-Root Execution**: Both services run as unprivileged users (`USER $APP_UID` in .NET, non-root user in Nginx on port 8080).
- **Privilege Restrictions**: All containers enforce `security_opt: [no-new-privileges:true]`.
- **Resource Limits**: Explicit memory constraints defined in `docker-compose.yml` (Postgres: 512MB, API: 384MB, Web: 128MB).
- **Nginx Security Headers**: Injected at proxy level (`X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Permissions-Policy`), plus Gzip compression and long-lived static asset caching.

---

## Project Structure

```text
library-challenge/
├── docs/
│   └── ARCHITECTURE.md            # System context, container diagrams, ERD, and sequence flows
├── scripts/
│   ├── init-secrets.sh            # Optional credential rotation for Linux / macOS
│   └── init-secrets.ps1           # Optional credential rotation for Windows PowerShell
├── secrets/
│   └── db_password.txt            # Pre-seeded development secret for Docker Compose
├── src/
│   ├── LibraryApi/                # .NET 10 Minimal API Backend
│   │   ├── Data/                  # DbContext, EF Configurations, Migrations, SeedData
│   │   ├── Domain/                # Entity models (Book, Author, Genre, ValueObjects)
│   │   ├── Features/              # Vertical slices (Authors, Books, Genres)
│   │   │   ├── Authors/           # CreateAuthor, DeleteAuthor, GetAuthorById, GetAuthors, UpdateAuthor
│   │   │   ├── Books/             # CreateBook, DeleteBook, GetBookById, GetBooks, UpdateBook
│   │   │   └── Genres/            # CreateGenre, DeleteGenre, GetGenreById, GetGenres, UpdateGenre
│   │   ├── Shared/                # Filters (ValidationFilter), Middleware, Extensions, Models
│   │   ├── Dockerfile             # Multi-stage build on .NET 10 SDK & chiseled runtime
│   │   └── Program.cs             # Application bootstrap & middleware pipeline
│   └── library-web/               # Angular 22 Single-Page Application
│       ├── src/app/
│       │   ├── components/        # Standalone components (book-list, book-form-modal, author-list, genre-list)
│       │   ├── models/            # TypeScript DTOs and contracts
│       │   └── services/          # Strongly-typed HTTP services (BookService, AuthorService, GenreService)
│       ├── Dockerfile             # Multi-stage build on Node 22 & Nginx unprivileged
│       ├── nginx.conf             # Reverse proxy & security configuration
│       ├── proxy.conf.json        # Local dev proxy configuration for ng serve
│       └── README.md              # Frontend architecture & guide
├── tests/
│   ├── LibraryApi.IntegrationTests/ # Integration tests with Testcontainers & WebApplicationFactory
│   └── LibraryApi.UnitTests/        # Domain, Validator, & Middleware unit tests
├── LibraryChallenge.slnx          # Solution file (.NET 10 / Visual Studio)
├── docker-compose.yml             # Container orchestration with secrets and resource limits
└── README.md
```

---

## REST API Reference

The interactive documentation and Swagger/OpenAPI specifications are hosted directly by the application:
- **Backend API Base URL**: [http://localhost:5000](http://localhost:5000) (direct) &bull; Reverse proxy: [http://localhost/api](http://localhost/api)
- **Interactive UI (Scalar)**: [http://localhost/scalar/v1](http://localhost/scalar/v1) (or `http://localhost:5000/scalar/v1` bare-metal)
- **OpenAPI 3.1 Spec**: [http://localhost/openapi/v1.json](http://localhost/openapi/v1.json) (or `http://localhost:5000/openapi/v1.json` bare-metal)


### Endpoints Summary

| Method | Route | Description | Query Parameters / Request Body | Status Codes |
| :--- | :--- | :--- | :--- | :--- |
| **GET** | `/api/books` | Get paginated list of books | `page`, `pageSize`, `searchTerm`, `genreId`, `authorId`, `sortBy` (`title`, `year`, `author`, `genre`), `sortDirection` (`asc`, `desc`) | `200 OK` |
| **GET** | `/api/books/{id}` | Get book by ID with author and genre details | `id: Guid` | `200 OK`, `404 Not Found` |
| **POST** | `/api/books` | Create a new book | Body: `CreateBookRequest` (`title`, `isbn`, `publishedYear`, `description?`, `authorId`, `genreId`) | `201 Created`, `400 Bad Request`, `409 Conflict`, `422 Unprocessable Entity` |
| **PUT** | `/api/books/{id}` | Update an existing book | Route: `id: Guid`<br>Body: `UpdateBookRequest` | `204 NoContent`, `400 Bad Request`, `404 Not Found`, `409 Conflict`, `422 Unprocessable Entity` |
| **DELETE** | `/api/books/{id}` | Delete a book | Route: `id: Guid` | `204 NoContent`, `404 Not Found` |
| **GET** | `/api/authors` | Get paginated list of authors with book counts | `page`, `pageSize`, `searchTerm`, `sortBy` (`name`), `sortDirection` (`asc`, `desc`) | `200 OK` |
| **GET** | `/api/authors/{id}` | Get author by ID with list of their books | `id: Guid` | `200 OK`, `404 Not Found` |
| **POST** | `/api/authors` | Create a new author | Body: `CreateAuthorRequest` (`name`) | `201 Created`, `400 Bad Request`, `409 Conflict` |
| **PUT** | `/api/authors/{id}` | Update an existing author | Route: `id: Guid`<br>Body: `UpdateAuthorRequest` (`name`) | `204 NoContent`, `400 Bad Request`, `404 Not Found`, `409 Conflict` |
| **DELETE** | `/api/authors/{id}` | Delete an author (blocked if author has books) | Route: `id: Guid` | `204 NoContent`, `404 Not Found`, `409 Conflict` |
| **GET** | `/api/genres` | Get paginated list of genres with book counts | `page`, `pageSize`, `searchTerm`, `sortBy` (`name`), `sortDirection` (`asc`, `desc`) | `200 OK` |
| **GET** | `/api/genres/{id}` | Get genre by ID with list of associated books | `id: Guid` | `200 OK`, `404 Not Found` |
| **POST** | `/api/genres` | Create a new genre | Body: `CreateGenreRequest` (`name`) | `201 Created`, `400 Bad Request`, `409 Conflict` |
| **PUT** | `/api/genres/{id}` | Update an existing genre | Route: `id: Guid`<br>Body: `UpdateGenreRequest` (`name`) | `204 NoContent`, `400 Bad Request`, `404 Not Found`, `409 Conflict` |
| **DELETE** | `/api/genres/{id}` | Delete a genre (blocked if genre has books) | Route: `id: Guid` | `204 NoContent`, `404 Not Found`, `409 Conflict` |
| **GET** | `/health` | Application health check | *None* | `200 OK` ("Healthy") |

---

## Getting Started

### Prerequisites

- **For Docker Compose (Recommended)**: [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine 20.10+ with Docker Compose v2.
- **For Bare-Metal Local Development**:
  - [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
  - [Node.js](https://nodejs.org/) 22+ & npm 10+
  - Docker daemon (required to run PostgreSQL locally or execute integration tests via Testcontainers)

---

### Using Docker Compose (Recommended)

1. **Start the containers** (zero-step setup — development secrets are pre-configured):
   ```bash
   docker compose up --build
   ```

2. **Access points**:
   - **Frontend Web UI**: [http://localhost](http://localhost) (port 80)
   - **Backend REST API**: [http://localhost:5000](http://localhost:5000) (direct) &bull; Reverse proxy: [http://localhost/api](http://localhost/api)
   - **Interactive API Docs (Scalar)**: [http://localhost/scalar/v1](http://localhost/scalar/v1)
   - **API Health Check**: [http://localhost/health](http://localhost/health)
   - **PostgreSQL Database**: `localhost:5432`

> 💡 **Credential Rotation (Optional)**: A default development secret (`secrets/db_password.txt`) is committed for instant zero-configuration evaluation. If you wish to rotate or generate a random high-entropy secret, run `./scripts/init-secrets.sh --force` (Linux/macOS) or `.\scripts\init-secrets.ps1 -Force` (Windows).

---

### Default Credentials & Seeded Data

| Target | Access / URLs | Details |
| :--- | :--- | :--- |
| **Frontend Web UI** | [http://localhost](http://localhost) (Docker, port 80)<br>[http://localhost:4200](http://localhost:4200) (Bare-Metal) | Angular 22 Single-Page Application (no login required, pre-seeded catalog). |
| **Backend REST API** | [http://localhost:5000](http://localhost:5000) (direct)<br>[http://localhost/api](http://localhost/api) (Docker proxy) | .NET 10 Minimal API endpoints (`/api/books`, `/api/authors`, `/api/genres`). |
| **Interactive API Docs** | [http://localhost/scalar/v1](http://localhost/scalar/v1)<br>[http://localhost:5000/scalar/v1](http://localhost:5000/scalar/v1) (bare-metal) | Scalar OpenAPI 3.1 interactive UI (no authentication required). |
| **PostgreSQL Database** | **Host:** `localhost` / `127.0.0.1`<br>**Port:** `5432`<br>**Database:** `librarydb`<br>**User:** `library`<br>**Password:** `library_secret` | Unified password across Docker Compose (`secrets/db_password.txt`) and bare-metal environments. |

#### Seeded Catalog
On initial migration, the database is pre-seeded with:
- **3 Genres**: Fiction, Science Fiction, Fantasy
- **4 Authors**: George Orwell, Isaac Asimov, J.R.R. Tolkien, F. Scott Fitzgerald
- **6 Books**:
  1. *1984* (George Orwell &bull; Fiction &bull; 1949)
  2. *Foundation* (Isaac Asimov &bull; Science Fiction &bull; 1951)
  3. *The Hobbit* (J.R.R. Tolkien &bull; Fantasy &bull; 1937)
  4. *The Great Gatsby* (F. Scott Fitzgerald &bull; Fiction &bull; 1925)
  5. *I, Robot* (Isaac Asimov &bull; Science Fiction &bull; 1950)
  6. *The Lord of the Rings* (J.R.R. Tolkien &bull; Fantasy &bull; 1954)

---

### Running Bare-Metal (Locally)

If you wish to run the app natively without Docker Compose:

1. **Start PostgreSQL Database**:
   ```bash
   docker run --name library-db -e POSTGRES_USER=library -e POSTGRES_PASSWORD=library_secret -e POSTGRES_DB=librarydb -p 5432:5432 -d postgres:17-alpine
   ```

2. **Start the Backend API**:
   ```bash
   cd src/LibraryApi
   dotnet run --environment Development
   ```
   *The API will be available at [http://localhost:5000](http://localhost:5000). Note: Database migrations and seed catalog data are automatically applied on startup (`app.ApplyMigrationsAsync()`).*

3. **Start the Angular Frontend**:
   ```bash
   cd src/library-web
   npm install
   npm start
   ```
   *The Web app will be available at [http://localhost:4200](http://localhost:4200). Requests to `/api/*`, `/scalar/*`, `/openapi/*`, and `/health` are automatically proxied to the backend at `http://localhost:5000` via [`proxy.conf.json`](src/library-web/proxy.conf.json).*

---

## Testing

### Backend Integration Tests
Integration tests run against a real PostgreSQL instance using `Testcontainers.PostgreSql` and `WebApplicationFactory<Program>`:

```bash
# Run all tests directly from the repository root
dotnet test

# Or run from the test project directory
cd tests/LibraryApi.IntegrationTests
dotnet test
```

> **Note**: Requires a running Docker daemon on the host machine to spin up the PostgreSQL test container.

### Frontend Unit Tests
Unit tests use [Vitest](https://vitest.dev/):

```bash
cd src/library-web
npm test
```

---

## Trade-offs and Limitations

- **Security & Authentication**: JWT / OAuth2 / OIDC authentication was intentionally omitted to focus on core domain modeling, vertical slice architecture, and security hardening at the container and transport layers.
- **Testing Scope**: Backend integration tests cover full database persistence, validation errors, and conflict scenarios. Frontend unit tests are scaffolded with Vitest.
- **Relational Integrity vs Soft Deletes**: Implemented strict foreign key constraints (`DeleteBehavior.Restrict`) and hard deletes. Attempting to delete an author or genre that currently has books is prevented at the database level and returns an RFC 7807 `409 Conflict`.
- **Pagination Strategy**: Uses offset pagination (`Skip`/`Take`) bounded by `MaxPageSize = 100`. For massive datasets (millions of rows), keyset (cursor-based) pagination would be preferred.
- **Projections over Eager Loading**: Database queries project directly to flat DTOs (`.Select(...)`) to eliminate N+1 queries and avoid transferring unneeded columns.
