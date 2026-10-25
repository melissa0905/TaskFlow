export interface NotificationListItem {
    id: string;
    taskId: string;
    message: string;
    isRead: boolean;
    createdAtUtc: string;
}