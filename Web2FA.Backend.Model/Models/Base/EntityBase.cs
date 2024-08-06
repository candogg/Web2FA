namespace Web2FA.Backend.Model.Models.Base
{
    public abstract class EntityBase
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
