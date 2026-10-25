namespace TaskFlow.Application.Notifications.Queries.GetNotifications;

public sealed record NotificationListItem(
    Guid Id,
    Guid TaskId,
    string Message,
    bool IsRead,
    DateTime CreatedAtUtc);