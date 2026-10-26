from typing import Literal
from uuid import UUID

from pydantic import BaseModel,ConfigDict,Field,field_validator

class CreateTaskPlanRequest(BaseModel):
    model_config = ConfigDict(
        str_strip_whitespace=True,
        extra="forbid"
    )
    project_id: UUID
    goal: str = Field(
        min_length=10,
        max_length=2000,
    )
    max_tasks: int = Field(
        default=5,
        ge=1,
        le=10,
        strict=True
    )
class TaskProposal(BaseModel):
    model_config = ConfigDict(
        str_strip_whitespace=True,
        extra="forbid",
    )

    title: str = Field(
        min_length=1,
        max_length=200,
    )

    description: str = Field(
        min_length=1,
        max_length=2000,
    )

    category: Literal["backend", "frontend", "testing", "devops"]


class TaskPlanResponse(BaseModel):
    model_config = ConfigDict(extra="forbid")

    project_id: UUID

    summary: str = Field(
        min_length=1,
        max_length=1000,
    )

    tasks: list[TaskProposal] = Field(
        min_length=1,
        max_length=10,
    )