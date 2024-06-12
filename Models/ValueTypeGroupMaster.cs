using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.InterviewPrep.PostgreSQL.Models
{
    public class ValueTypeGroupMaster : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ValueTypeMaster> ValueTypeMasters { get; set; }
    }
}
