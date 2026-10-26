from fastapi import FastAPI
from app.api.routes.health import router as health_router

app = FastAPI(
    title="TaskFlow AI Service",
    description="TaskFlow için görev planlama ve AI agent servisi",
    version="0.1.0"
)
app.include_router(health_router)