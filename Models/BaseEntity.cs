namespace Core.InterviewPrep.PostgreSQL.Models
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }= DateTime.UtcNow;
        public string CreatedBy { get; set; } = "1";
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
