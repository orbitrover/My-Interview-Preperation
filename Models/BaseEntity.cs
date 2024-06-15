namespace Core.InterviewPrep.PostgreSQL.Models
{
    public class BaseEntity
    {
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public string CreatedBy { get; set; } = "1";
        public DateTimeOffset? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
