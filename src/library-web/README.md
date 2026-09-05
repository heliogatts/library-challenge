# LibraryWeb — Angular Frontend

A modern Single-Page Application (SPA) for the **Library Management System**, built with Angular 22, Standalone Components, and Angular Signals.

---

## 1. Overview & Tech Stack

- **Framework**: [Angular 22](https://angular.dev/)
- **State & Reactivity**: Native Angular Signals (`signal()`, `computed()`, `effect()`, `input()`, `output()`)
- **Architecture**: Standalone Components (no `NgModule` boilerplate)
- **HTTP Client**: Strongly-typed Angular `HttpClient` with `withFetch()` enabled
- **Styling**: Pure CSS using CSS custom properties, design tokens, and responsive layout
- **Test Runner**: [Vitest](https://vitest.dev/)
- **Production Server**: Nginx Unprivileged (`nginxinc/nginx-unprivileged:alpine`)

---

## 2. Project Structure

```text
src/app/
├── components/
│   ├── book-list/            # Book catalog table with search, filters, pagination, and actions
│   ├── book-form-modal/      # Modal dialog for creating and updating books
│   ├── author-list/          # Author management with book counts, search, and inline creation
│   └── genre-list/           # Genre management with book counts, search, and inline creation
├── models/
│   ├── author.model.ts       # Author DTOs and view models
│   ├── book.model.ts         # Book DTOs, request payloads, and detail models
│   ├── genre.model.ts        # Genre DTOs and view models
│   └── paged-response.model.ts # Generic pagination contract matching backend PagedResponse<T>
├── services/
│   ├── author.service.ts     # Author CRUD HTTP operations
│   ├── book.service.ts       # Book catalog query and mutation HTTP operations
│   └── genre.service.ts      # Genre CRUD HTTP operations
├── app.config.ts             # Application routing, fetch client, and global error handling
├── app.routes.ts             # Client-side route declarations (/books, /authors, /genres)
├── app.html                  # Top-level shell with navigation bar and router outlet
├── app.ts                    # Root standalone component
└── app.css                   # Header, navigation, and shell styling
```

---

## 3. Key Features

- **Books Catalog (`/books`)**:
  - Full-text search by title (case-insensitive via ILIKE on backend).
  - Dynamic filtering by Author and Genre dropdowns.
  - Interactive multi-column sorting (Title, Year, Author, Genre) with direction toggles (▲ / ▼).
  - Server-side pagination with previous/next controls and total records counter.
  - Inline action buttons to view details, edit existing books, or delete books.
- **Book Modal (`BookFormModalComponent`)**:
  - Reusable modal dialog for both adding new books and editing existing books.
  - Automatically loads full book details when an ID is supplied.
  - Real-time client validation (title, ISBN formatting with hyphen support up to 17 chars, published year, author, genre).
  - Displays RFC 7807 error messages returned by the API (e.g., duplicate ISBN conflicts).
- **Authors Management (`/authors`)**:
  - Live search filtering by author name.
  - Paginated author list showing the number of books assigned to each author.
  - Add author form with immediate feedback.
  - Inline author editing with instant save and cancel operations.
  - Safe delete handling: displays a 409 Conflict alert if attempting to delete an author with existing books.
- **Genres Management (`/genres`)**:
  - Live search filtering by genre name.
  - Paginated genre list showing the number of books assigned to each genre.
  - Add genre form with duplicate name validation.
  - Inline genre editing with instant save and cancel operations.
  - Safe delete handling: displays a 409 Conflict alert if attempting to delete a genre with existing books.

---

## 4. Local Development

### Prerequisites
- Node.js 22+
- npm 10+
- The backend API running on `http://localhost:5000` (see root [README.md](../../README.md))

### Installation
```bash
npm install
```

### Start Development Server
```bash
npm start
```
*or directly with the Angular CLI:*
```bash
ng serve --proxy-config proxy.conf.json --port 4200
```

The application will be accessible at [http://localhost:4200](http://localhost:4200).

### Backend Proxy Configuration
During local development with `ng serve`, API requests are proxied to `http://localhost:5000` via [`proxy.conf.json`](proxy.conf.json):
- `/api/*` &rarr; `http://localhost:5000/api/*`
- `/scalar/*` &rarr; `http://localhost:5000/scalar/*`
- `/openapi/*` &rarr; `http://localhost:5000/openapi/*`
- `/health` &rarr; `http://localhost:5000/health`

This avoids CORS issues and replicates the Docker Compose Nginx reverse-proxy setup.

---

## 5. Building for Production

To compile the application:
```bash
npm run build
```

This compiles the TypeScript and templates into optimized static bundles located in `dist/library-web/browser`.

---

## 6. Testing

Unit tests are configured with [Vitest](https://vitest.dev/):
```bash
npm test
```

---

## 7. Containerization

The production Docker container uses a multi-stage build:
1. **Build Stage (`node:22-alpine`)**: Restores dependencies via `npm ci` and runs `npm run build -- --configuration production`.
2. **Runtime Stage (`nginxinc/nginx-unprivileged:alpine`)**: Copies static artifacts to `/usr/share/nginx/html` and applies [`nginx.conf`](nginx.conf) with:
   - Security headers (`X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`).
   - Gzip compression for text, scripts, JSON, and stylesheets.
   - Long-lived caching headers (`max-age=31536000, immutable`) for hashed assets.
   - Reverse-proxy directives for `/api/`, `/scalar/`, `/openapi/`, and `/health`.
