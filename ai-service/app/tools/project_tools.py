from uuid import UUID

import httpx

from app.core.config import get_settings
from app.schemas.project_context import ProjectContext


class ProjectNotFoundError(Exception):
    pass

class ProjectApiError(Exception):
    pass

async def get_project(project_id : UUID) -> ProjectContext:
    settings = get_settings()

    try:
        async with httpx.AsyncClient(
            base_url= settings.dotnet_api_base_url,
            timeout=10.0,
        )as client:
            response = await client.get(f"/api/projects/{project_id}")
        if response.status_code == 404:
            raise ProjectNotFoundError("Seçilen Proje bulunamadı.")
        response.raise_for_status()

        project= ProjectContext.model_validate( response.json() )

        if project_id != project_id:
            raise ProjectApiError("API farklı bir projenin bilgisini döndürdü.")
        return project
    except(httpx.HTTPError,ValueError) as exception:
        raise ProjectApiError(
            "Proje bilgisi .NET API den alınamadı."
        )from exception
