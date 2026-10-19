using MassTransit;
using TaskFlow.Contracts.Events;

namespace TaskFlow.Worker.Consumers;

public sealed class WorkTaskAssignedConsumer(ILogger<WorkTaskAssignedConsumer> logger) : IConsumer<WorkTaskAssigned>
{
    public Task Consume(ConsumeContext<WorkTaskAssigned> context)
    {
        var message = context.Message;
        logger.LogInformation(
          "Görev atama olayı alındı. EventId: {EventId}, " +
          "TaskId: {TaskId}, AssigneeId: {AssigneeId}, " +
          "Title: {Title}",
          message.EventId,
          message.TaskId,
          message.AssignedId,
          message.Title);
        return Task.CompletedTask;
    }
}
