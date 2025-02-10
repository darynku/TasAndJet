namespace SharedKernel.Paged;

public class PagedList<T>
{ 
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public IReadOnlyList<T> Items { get; set; } = [];
    
    public bool HasPreviousPage => Page > 1;
    
    public bool HasNextPage => Page * PageSize < TotalCount;
}