namespace TaskFlow.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages =>
        (int)Math.Ceiling(TotalCount / (double)PageSize);
}