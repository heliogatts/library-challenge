namespace LibraryApi.Shared.Models;

public record PagedRequest
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    private int _page = DefaultPage;
    public int? Page
    {
        get => _page;
        init => _page = value.HasValue ? Math.Max(1, value.Value) : DefaultPage;
    }

    private int _pageSize = DefaultPageSize;
    public int? PageSize
    {
        get => _pageSize;
        init => _pageSize = value.HasValue ? Math.Clamp(value.Value, 1, MaxPageSize) : DefaultPageSize;
    }

    public string? SearchTerm { get; init; }
    
    private string? _sortBy;
    public string? SortBy { get => _sortBy ?? "name"; init => _sortBy = value; }
    
    private string? _sortDirection;
    public string? SortDirection { get => _sortDirection ?? "asc"; init => _sortDirection = value; }
}
