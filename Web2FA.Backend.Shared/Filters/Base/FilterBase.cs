namespace Web2FA.Backend.Shared.Filters.Base
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public abstract class FilterBase
    {
        public string? SearchKeyword { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }
    }
}
