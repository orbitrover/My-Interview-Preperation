using Core.InterviewPrep.PostgreSQL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.InterviewPrep.PostgreSQL.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<ValueTypeGroupMaster> ValueTypeGroupMaster { get; set; }
        public DbSet<ValueTypeMaster> ValueTypeMaster { get; set; }
        public DbSet<Headings> Headings { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Answers> Answers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //------------Seeding---------------//
            builder.Entity<ValueTypeGroupMaster>().HasData(
             new ValueTypeGroupMaster
             { Id = 1, Name = "Topics", Description = "Topics" },
             new ValueTypeGroupMaster
             { Id = 2, Name = "Headings", Description = "Headings" },
             new ValueTypeGroupMaster
             { Id = 3, Name = "Questions", Description = "Questions" },
             new ValueTypeGroupMaster
             { Id = 4, Name = "Answers", Description = "Answers" });
            builder.Entity<ValueTypeMaster>().HasData(
            new ValueTypeMaster { Id = 1001, ValueTypeGroupId = 1, Name = "Personal Introduction", Description = "Personal Introduction" },
            new ValueTypeMaster { Id = 1002, ValueTypeGroupId = 1, Name = "Skill Introduction", Description = "Skill Introduction" },
            new ValueTypeMaster { Id = 1003, ValueTypeGroupId = 1, Name = "Project Introduction", Description = "Project Introduction" },
            new ValueTypeMaster { Id = 1004, ValueTypeGroupId = 1, Name = "Tech Interview questions", Description = "Tech Interview questions" },
            new ValueTypeMaster { Id = 1005, ValueTypeGroupId = 1, Name = "behavioral Interview questions", Description = "behavioral interview questions" }
            );
        }
    }
}
