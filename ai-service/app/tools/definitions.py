from uuid import UUID

from google.genai import types
from pydantic import BaseModel, ConfigDict


class GetProjectArguments(BaseModel):
    model_config = ConfigDict(extra="forbid")

    project_id: UUID


GET_PROJECT_TOOL = types.Tool(
    function_declarations=[
        types.FunctionDeclaration(
            name="get_project",
            description=(
                "Seçilen projenin adını ve açıklamasını "
                "TaskFlow API üzerinden getirir."
            ),
            parameters_json_schema={
                "type": "object",
                "properties": {
                    "project_id": {
                        "type": "string",
                        "description": (
                            "İstekte verilen projenin UUID kimliği."
                        ),
                    }
                },
                "required": ["project_id"],
                "additionalProperties": False,
            },
        )
    ]
)