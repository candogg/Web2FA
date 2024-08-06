namespace Web2FA.Backend.Shared.Dto.Base
{
    public abstract class DtoBase
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
