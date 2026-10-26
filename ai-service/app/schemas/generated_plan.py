from pydantic import BaseModel,ConfigDict,Field
from app.schemas.task_plan import TaskProposal

class GeneratedTaskPlan(BaseModel):
    model_config = ConfigDict(
        str_strip_whitespace=True,
        extra="forbid",
    )
    summary: str = Field(
        min_length=1,
        max_length=1000,
    )
    tasks: list[TaskProposal] = Field(
        min_length=1,
        max_length=10
    )