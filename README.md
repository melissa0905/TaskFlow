# TaskFlow — AI-Powered Project and Task Management

TaskFlow is a project management application built with React, ASP.NET Core, and Python, featuring **Google Gemini-powered task planning**. Manage projects and tasks, then turn a goal into actionable task suggestions based on your project's details.

## From Goals to Task Plans with AI

The Python/FastAPI service uses Gemini function calling to retrieve the selected project's details from the .NET API and generate a plan tailored to the user's goal.

1. The user submits a project ID, a goal, and the maximum number of tasks to generate.
2. Gemini calls the `get_project` tool. Python verifies that the requested ID matches the selected project and retrieves its name and description from the .NET API.
3. The tool result is passed to Gemini, which generates a summary and task suggestions in Turkish.
4. Pydantic validates the response before it is returned as JSON.

Each suggestion includes a title, a description, and a category: `backend`, `frontend`, `testing`, or `devops`. Requests can specify between 1 and 10 tasks. The service uses the project name and description; it does not read source code or existing tasks. Generated plans are returned as suggestions and do not automatically create tasks in the database.

The frontend includes an AI planner page that sends requests through the `/ai` proxy. You can also call the AI service directly using its HTTP API, as shown below.

## Technology and Architecture

| Layer | Technologies | Responsibility |
| --- | --- | --- |
| Frontend | React 19, TypeScript, Vite | User interface |
| Backend | ASP.NET Core, .NET 10, EF Core, PostgreSQL | Project and task data |
| AI service | Python, FastAPI, Google Gen AI SDK, Pydantic | Gemini tool calls and validated task plans |
| Messaging | RabbitMQ, MassTransit, SignalR | Event processing and live notifications |

```text
TaskFlow/
  ai-service/  Python + FastAPI + Gemini task planning service
  backend/     ASP.NET Core API, application, domain, infrastructure, and worker
  frontend/    React + TypeScript + Vite client
```

## Local Setup

Prerequisites: .NET 10 SDK, Docker, Node.js/npm, Python 3.11 or later, and a Gemini API key. Start each setup section from the repository root and use separate terminals for the backend, AI service, and frontend.

### 1. Backend and Infrastructure

Start PostgreSQL and RabbitMQ:

```powershell
cd backend
docker compose up -d
```

Before starting the API, configure the `ConnectionStrings:TaskFlowDb` connection string using user secrets or local configuration. Ensure that the database schema has been created using EF Core migrations.

```powershell
dotnet run --project src/TaskFlow.Api
```

The development HTTP address is `http://localhost:5042`.

### 2. Gemini AI Service

On Windows / PowerShell:

```powershell
cd ai-service
python -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r requirements.txt
Copy-Item app/.env.example .env
```

Skip the copy step if you already have a `.env` file. The service reads **`ai-service/.env`**. Set the following values in that file:

```dotenv
GEMINI_API_KEY=your_gemini_api_key
GEMINI_MODEL=gemini-3.8-flash
DOTNET_API_BASE_URL=http://127.0.0.1:5042
```

`GEMINI_MODEL` must be a model ID your account can access. If no value is provided, the code defaults to `gemini-2.5-flash`; the example environment file specifies `gemini-3.8-flash`. Keep your API key out of version control. Restart the AI service after changing its settings.

To match the frontend's current `/ai` proxy target, start the service on port `58147`:

```powershell
.\.venv\Scripts\python.exe -m uvicorn app.main:app --reload --port 58147
```

- Swagger UI: `http://127.0.0.1:58147/docs`
- Health check: `GET http://127.0.0.1:58147/health`
- Task planning: `POST http://127.0.0.1:58147/api/task-plans`

The health check confirms that the service is running; it does not test connectivity to Gemini or the .NET API. If you use a different port, update the `/ai` proxy target in `frontend/vite.config.ts`.

### 3. Frontend

```powershell
cd frontend
npm install
npm run dev
```

Open the URL printed by Vite in your browser. The frontend forwards `/api` and `/hubs` requests to the .NET API at `http://localhost:5042`. Requests under `/ai` are forwarded to `http://127.0.0.1:58147` with the `/ai` prefix removed.

## AI Task Planning Example

First, obtain an existing project ID from the .NET API's `GET /api/projects` response. In the AI service's Swagger UI, use the following body for `POST /api/task-plans`, replacing `project_id` with the actual project ID:

```json
{
  "project_id": "00000000-0000-0000-0000-000000000001",
  "goal": "Improve the task creation and listing workflow for the project.",
  "max_tasks": 3
}
```

The response follows the structure below. The example content is translated into English for readability; the service currently instructs Gemini to respond in Turkish. Content and task count vary by model and request.

```json
{
  "project_id": "00000000-0000-0000-0000-000000000001",
  "summary": "An implementation plan for task creation and listing.",
  "tasks": [
    {
      "title": "Implement task creation validation",
      "description": "Validate required fields. Invalid requests should return clear errors, and valid requests should be saved.",
      "category": "backend"
    }
  ]
}
```

## Troubleshooting AI Requests

| HTTP status | Possible cause |
| --- | --- |
| `404` | The selected project was not found. |
| `422` | Request validation failed. Check the UUID, goal length, and `max_tasks` value. |
| `502` | A .NET or Gemini connection failed, Gemini rejected the request, or the generated plan failed validation. Check the response's `detail` field and service logs. |
| `503` | The API key is missing, model access is unavailable, a quota or rate limit has been reached, or Gemini is busy. The `detail` field explains the cause. |
| `504` | Plan generation or the Gemini request timed out. |

A busy response from Gemini does not necessarily mean your API key is incorrect. The .NET API must also be running so the service can retrieve project details.

## Development Commands

Run these commands from the `frontend` directory:

```powershell
npm run build
npm run lint
```

Build artifacts and dependency directories such as `bin`, `obj`, `dist`, and `node_modules` are excluded from version control.
