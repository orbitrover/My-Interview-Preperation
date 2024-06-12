using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.InterviewPrep.PostgreSQL.Models
{
    public class ValueTypeMaster : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ValueTypeGroupId { get; set; }

        [ForeignKey("ValueTypeGroupId")]
        public virtual ValueTypeGroupMaster ValueTypeGroupMaster { get; set; }
        public virtual ICollection<Headings> Headings { get; set; }
    }

    //public class Topics : BaseEntity
    //{
    //    public int TopicId { get; set; }
    //    public string TopicName { get; set; }
    //    public string? Description { get; set; }
    //    public virtual ICollection<Questions> Questions { get; set; }
    //}
    public class Headings : BaseEntity
    {
        [Key]
        public int HeadingId { get; set; }
        public int TopicId { get; set; }
        public string HeadingName { get; set; }
        public virtual ICollection<Questions> Questions { get; set; }
        [ForeignKey("TopicId")]
        public virtual ValueTypeMaster Topic { get; set; }
    }
    public class Questions : BaseEntity
    {
        [Key]
        public int QuestionId { get; set; }
        public int HeadingId { get; set; }
        public string QuestionName { get; set; }
        public virtual ICollection<Answers> Answers { get; set; }
        [ForeignKey("HeadingId")]
        public virtual Headings Heading { get; set; }

    }
    public class Answers : BaseEntity
    {
        [Key]
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerName { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Questions Question { get; set; }
    }
}
