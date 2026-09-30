# TaskFlow

TaskFlow is a full-stack task management project with a React/Vite frontend and an ASP.NET Core backend.

## Repository Structure

```text
TaskFlow/
  backend/    ASP.NET Core API, application, domain, and infrastructure projects
  frontend/   React + TypeScript + Vite client
```

## Backend

Requirements:

- .NET 10 SDK
- Docker, for the local PostgreSQL database

Start PostgreSQL:

```bash
cd backend
docker compose up -d
```

Run the API:

```bash
cd backend
dotnet run --project src/TaskFlow.Api
```

The API project uses Entity Framework Core with PostgreSQL. Add the `TaskFlowDb` connection string through user secrets or local configuration before running the API.

## Frontend

Requirements:

- Node.js
- npm

Install dependencies and start the dev server:

```bash
cd frontend
npm install
npm run dev
```

Useful frontend commands:

```bash
npm run build
npm run lint
```

## Notes

- Keep both `frontend` and `backend` in this root repository.
- Build outputs such as `bin`, `obj`, `dist`, and `node_modules` are intentionally ignored.
