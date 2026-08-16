namespace ResourceManagerAPI.Models
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTimeOffset CreatedAt { get; protected set; }
        public string? CreatedBy { get; protected set; }

        public DateTimeOffset? LastModifiedAt { get; protected set; }
        public string? LastModifiedBy { get; protected set; }

        public void SetCreated(string? UserId)
        {
            CreatedAt = DateTimeOffset.UtcNow;
            CreatedBy = UserId;
        }
        public void SetModified(string? UserId)
        {
            LastModifiedAt = DateTimeOffset.UtcNow;
            LastModifiedBy = UserId;
        }
    }
}
