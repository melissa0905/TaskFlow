from fastapi import APIRouter, HTTPException,status
from app.schemas.task_plan import(
    CreateTaskPlanRequest,
    TaskPlanResponse,
)

router = APIRouter(
    prefix="/api/task-plans",
    tags=["Task Plans"],
)

@router.post(
    "",
    response_model=TaskPlanResponse,
    responses={
        501:{
            "description": "Model bağlantısı henüz eklenmedi."
        }
    },
)
async def create_task_plan(
    request: CreateTaskPlanRequest
)-> TaskPlanResponse:
    raise HTTPException(
        status_code=status.HTTP_501_NOT_IMPLEMENTED,
        detail="Görev planlama için model bağlantısı henüz eklenmedi.",
    )