import logging
from typing import Annotated

from fastapi import APIRouter, Depends, HTTPException,status
from openai import(APIConnectionError,APIStatusError,APITimeoutError,AuthenticationError,RateLimitError)
from pydantic import ValidationError

from app.schemas.task_plan import(
    CreateTaskPlanRequest,
    TaskPlanResponse,
)

from app.services.task_plan_generator import(
    ModelNotConfiguredError,
    PlanGenerationError,
    TaskPlanGenerator,
)
from app.tools.project_tools import(
    ProjectApiError,
    ProjectNotFoundError
)

logger = logging.getLogger(__name__)
router = APIRouter(
    prefix="/api/task-plans",
    tags=["Task Plans"],
)

@router.post(
    "",
    response_model=TaskPlanResponse,
)
async def create_task_plan(
    request: CreateTaskPlanRequest,
    generator: Annotated[ TaskPlanGenerator, Depends(TaskPlanGenerator)],
)-> TaskPlanResponse:
    try:
        return await generator.generate(request)
    except ModelNotConfiguredError:
     raise HTTPException(
        status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
        detail="AI sağlayıcısı kimlik doğrulaması başarısız.",
    ) from None
    
    except RateLimitError as exception:
        logger.warning(
        "OpenAI 429: code=%s, type=%s",
        exception.code,
        exception.type,
    )
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="AI sağlayıcısının kota veya istek sınırına ulaşıldı.",
        ) from None
    except APITimeoutError:
        raise HTTPException(
            status_code=status.HTTP_504_GATEWAY_TIMEOUT,
            detail="AI yanıtı beklenirken zaman aşımı oluştu.",
        ) from None

    except APIConnectionError:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="AI sağlayıcısına bağlanılamadı.",
        ) from None

    except APIStatusError as exception:
        logger.error(
            "AI sağlayıcısı HTTP hatası döndürdü: %s",
            exception.status_code,
        )

        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="AI sağlayıcısı isteği tamamlayamadı.",
        ) from None

    except (PlanGenerationError, ValidationError):
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="Geçerli bir görev planı üretilemedi.",
        ) from None
    except ProjectNotFoundError:
        raise HTTPException(
        status_code=status.HTTP_404_NOT_FOUND,
        detail="Seçilen proje bulunamadı.",
    ) from None

    except ProjectApiError:
        raise HTTPException(
        status_code=status.HTTP_502_BAD_GATEWAY,
        detail="Proje bilgisi .NET API'den alınamadı.",
    ) from None
    