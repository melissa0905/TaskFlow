import type { NotificationListItem } from "../types/notification";
import type { PagedResult } from "../types/pagedResult";
import { readApiError } from "./ApiError";


type GetNotificationsParams = {
    recipientId: string;
    onlyUnread: boolean;
    page: number;
    pageSize: number;
}

export async function getNotifications(
  {
    recipientId,
    onlyUnread,
    page,
    pageSize,
  }: GetNotificationsParams,
  signal?: AbortSignal,
): Promise<PagedResult<NotificationListItem>> {
  const params = new URLSearchParams({
    recipientId,
    onlyUnread: String(onlyUnread),
    page: String(page),
    pageSize: String(pageSize),
  });

  const response = await fetch(
    `/api/notifications?${params.toString()}`,
    { signal },
  );

  if (!response.ok) {
    throw await readApiError(
      response,
      'Bildirimler yüklenemedi.',
    );
  }

  return response.json();
}

export async function markNotificationAsRead(
  id: string,
  recipientId: string,
): Promise<void> {
  const params = new URLSearchParams({ recipientId });

  const response = await fetch(
    `/api/notifications/${id}/read?${params.toString()}`,
    { method: 'PATCH' },
  );

  if (!response.ok) {
    throw await readApiError(
      response,
      'Bildirim okundu olarak işaretlenemedi.',
    );
  }
}