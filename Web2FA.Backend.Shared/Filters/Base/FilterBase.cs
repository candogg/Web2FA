namespace Web2FA.Backend.Shared.Filters.Base
{
    public abstract class FilterBase
    {
        public string? SearchKeyword { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }
    }
}
