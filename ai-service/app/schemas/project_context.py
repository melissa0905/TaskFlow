from uuid import UUID
from pydantic import BaseModel

class ProjectContext(BaseModel):
    id: UUID
    name: str
    description: str | None = None