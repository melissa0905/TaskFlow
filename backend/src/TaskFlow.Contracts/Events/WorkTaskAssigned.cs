namespace TaskFlow.Contracts.Events;

public sealed record WorkTaskAssigned(
    ///<summary>
    /// Unique identifier of the event.
    ///</summary>
    Guid EventId,
    ///<summary>
    /// Unique identifier of the work task.
    /// </summary>
    Guid TaskId,
    ///<summary>
    /// Unique identifier of the user to whom the task is assigned.
    /// </summary>
    Guid AssignedId,
    ///<summary>
    /// Title of the work task.
    /// </summary>
    string Title,
    ///<summary>
    /// UTC timestamp when the event occurred.
    /// </summary>
    DateTime OccuredAtUtc
);
