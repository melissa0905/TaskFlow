namespace TaskFlow.Application.Projects.Queries.GetDashboardSummary;

public sealed record DashboardSummary(
    int TotalTasks,
    int InProgressTasks,
    int CompletedTasks,
    int OverdueTasks);