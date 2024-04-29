namespace ContentService.Helpers
{
    public class PostSearchParams: SearchParams
    {
        public PostFilters? PostFilters { get; set; } = null;
    }
}

public enum PostFilters
{
    explore, following
}
