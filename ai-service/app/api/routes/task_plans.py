import logging
from typing import Annotated

import httpx
from fastapi import APIRouter, Depends, HTTPException, status
from google.genai import errors
from pydantic import ValidationError

from app.schemas.task_plan import (
    CreateTaskPlanRequest,
    TaskPlanResponse,
)
from app.services.task_plan_generator import (
    ModelNotConfiguredError,
    PlanGenerationError,
    TaskPlanGenerator,
)
from app.tools.project_tools import (
    ProjectApiError,
    ProjectNotFoundError,
)


logger = logging.getLogger(__name__)

router = APIRouter(
    prefix="/api/task-plans",
    tags=["Task Plans"],
)


@router.post("", response_model=TaskPlanResponse)
async def create_task_plan(
    request: CreateTaskPlanRequest,
    generator: Annotated[
        TaskPlanGenerator,
        Depends(TaskPlanGenerator),
    ],
) -> TaskPlanResponse:
    try:
        return await generator.generate(request)

    except ModelNotConfiguredError:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="Gemini API anahtarı ayarlanmamış.",
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

    except (TimeoutError, httpx.TimeoutException):
        raise HTTPException(
            status_code=status.HTTP_504_GATEWAY_TIMEOUT,
            detail="AI yanıtı beklenirken zaman aşımı oluştu.",
        ) from None

    except errors.APIError as exception:
        logger.warning(
            "Gemini hatası: code=%s, message=%s",
            exception.code,
            exception.message,
        )

        if exception.code == 429:
            detail = (
                "Gemini ücretsiz kullanım kotasına veya "
                "istek sınırına ulaşıldı."
            )
            http_status = status.HTTP_503_SERVICE_UNAVAILABLE

        elif exception.code == 503:
            detail = (
                "Gemini modeli şu anda yoğun. "
                "Lütfen biraz sonra tekrar deneyin."
            )
            http_status = status.HTTP_503_SERVICE_UNAVAILABLE

        elif exception.code == 504:
            detail = "Gemini yanıtı beklenirken zaman aşımı oluştu."
            http_status = status.HTTP_504_GATEWAY_TIMEOUT

        elif exception.code in (401, 403):
            detail = (
                "Gemini anahtarı veya erişim izinleri "
                "kontrol edilmeli."
            )
            http_status = status.HTTP_503_SERVICE_UNAVAILABLE

        elif exception.code == 404:
            detail = (
                "Seçilen Gemini modeli bulunamadı "
                "veya bu hesap için kullanılamıyor."
            )
            http_status = status.HTTP_503_SERVICE_UNAVAILABLE

        else:
            detail = (
                "Gemini isteği tamamlayamadı. "
                "Model ve API ayarlarını kontrol et."
            )
            http_status = status.HTTP_502_BAD_GATEWAY

        raise HTTPException(
            status_code=http_status,
            detail=detail,
        ) from None

    except httpx.RequestError:
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="Gemini servisine bağlanılamadı.",
        ) from None

    except (PlanGenerationError, ValidationError):
        raise HTTPException(
            status_code=status.HTTP_502_BAD_GATEWAY,
            detail="Geçerli bir görev planı üretilemedi.",
        ) from None
