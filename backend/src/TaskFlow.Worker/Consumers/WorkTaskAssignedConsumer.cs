using MassTransit;
using TaskFlow.Application.Abstractions;
using TaskFlow.Contracts.Events;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Worker.Consumers;

public sealed class WorkTaskAssignedConsumer(
    ITaskNotificationRepository repository,
    ILogger<WorkTaskAssignedConsumer> logger)
    : IConsumer<WorkTaskAssigned>
{
    public async Task Consume(
        ConsumeContext<WorkTaskAssigned> context)
    {
        var message = context.Message;

        if (message.EventId == Guid.Empty ||
            message.TaskId == Guid.Empty ||
            message.AssignedId == Guid.Empty ||
            string.IsNullOrWhiteSpace(message.Title) ||
            message.Title.Length > 200)
        {
            throw new InvalidOperationException(
                "Görev atama mesajının alanları geçersiz.");
        }

        var notification = new TaskNotification
        {
            EventId = message.EventId,
            TaskId = message.TaskId,
            ReciptientId = message.AssignedId,
            Message = $"Sana '{message.Title}' görevi atandı.",
            IsRead = false,
            CreatedUtc = DateTime.UtcNow
        };

        var created = await repository.AddIfNotExistsAsync(
            notification,
            context.CancellationToken);

        if (!created)
        {
            logger.LogInformation(
                "Olay daha önce işlendi. EventId: {EventId}",
                message.EventId);

            return;
        }
        await context.Publish(
        new NotificationCreated(
            SourceEventId: message.EventId,
            RecipientId: message.AssignedId),
        context.CancellationToken);

        logger.LogInformation(
            "Bildirim oluşturuldu. EventId: {EventId}, " +
            "RecipientId: {RecipientId}",
            message.EventId,
            message.AssignedId);
    }
}