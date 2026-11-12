# TaskFlow Frontend

TaskFlow is a project and task management application. This repository contains its React and TypeScript frontend, built with Vite. It connects to the TaskFlow backend for application data and real-time notifications, and to a separate AI service for task planning.

## Features

- Account registration, login, and logout with cookie-based sessions.
- A dashboard with a summary of project and task activity.
- Project and team member management.
- Task creation, editing, deletion, status updates, and detail views.
- Task search, status filtering, and pagination.
- AI-assisted task planning based on a project and a goal.
- Notifications with live updates through SignalR.

## Tech Stack

- React 19 and TypeScript
- Vite 8
- React Router
- Microsoft SignalR
- ESLint

## Getting Started

### Prerequisites

- A Node.js version compatible with the Vite version in `package.json`, and npm.
- The TaskFlow backend running at `http://localhost:5042`.
- The AI planning service running at `http://127.0.0.1:58147` to use the AI planner.

The backend and AI service must be started separately; this repository's npm scripts run only the frontend.

### Installation

From the frontend directory, install the dependencies:

```sh
npm ci
```

Start the development server:

```sh
npm run dev
```

Open the local URL printed by Vite in your browser.

## Development Proxy

The frontend uses relative request URLs. The development server forwards requests using the proxy settings in `vite.config.ts`:

| Frontend path | Target | Notes |
| --- | --- | --- |
| `/api` | `http://localhost:5042` | Backend API, including authentication. |
| `/hubs` | `http://localhost:5042` | SignalR connections with WebSocket support. |
| `/ai` | `http://127.0.0.1:58147` | AI service; the `/ai` prefix is removed before forwarding. |

For example, `/ai/api/task-plans` is forwarded to `http://127.0.0.1:58147/api/task-plans`.

If your services use different addresses, update the proxy targets in `vite.config.ts`.

## Available Commands

| Command | Description |
| --- | --- |
| `npm run dev` | Start the Vite development server. |
| `npm run build` | Run TypeScript build checks and generate a production build in `dist/`. |
| `npm run lint` | Run ESLint. |
| `npm run preview` | Preview the production build locally after building. |

## Project Structure

```text
src/
  api/          API clients and error handling
  assets/       Images and static assets used by components
  auth/         Authentication context, session handling, and route guards
  components/   Shared UI components
  hooks/        Task, AI planning, notification, and utility hooks
  pages/        Application pages
  types/        Shared TypeScript types
  App.tsx       Application layout and routes
  main.tsx      Application entry point
```

## Production Deployment

Build the frontend with `npm run build` and serve the generated `dist/` directory with a static web server.

Vite's development proxy is not included in the production build. Configure your hosting server or reverse proxy to forward `/api`, `/hubs`, and `/ai` to the corresponding services, preserving the AI path rewrite and WebSocket support described above. Serve `index.html` for application routes so direct navigation and page refreshes work with React Router.
