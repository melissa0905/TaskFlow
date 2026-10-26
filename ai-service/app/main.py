from fastapi import FastAPI
from app.api.routes.health import router as health_router
from app.api.routes.task_plans import router as task_plans_router

app = FastAPI(
    title="TaskFlow AI Service",
    description="TaskFlow için görev planlama ve AI agent servisi",
    version="0.1.0"
)
app.include_router(health_router)
app.include_router(task_plans_router)