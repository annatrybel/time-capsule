using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TimeCapsule.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using TimeCapsule.Models.Dto;

namespace TimeCapsule.Models
{
    public class TimeCapsuleContext : IdentityDbContext<IdentityUser>
    {
        public TimeCapsuleContext(DbContextOptions options) : base(options) { }

        public DbSet<Capsule> Capsules { get; set; }
        public DbSet<CapsuleImage> CapsuleImages { get; set; }
        public DbSet<CapsuleQuestion> CapsuleQuestions { get; set; }
        public DbSet<CapsuleAnswer> CapsuleAnswers { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<CapsuleSection> CapsuleSections { get; set; }
        public DbSet<CapsuleLink> CapsuleLinks { get; set; }
        public DbSet<CapsuleRecipient> CapsuleRecipients { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDto>().HasNoKey().ToView("UserWithRoles");

            modelBuilder.Entity<Capsule>()
                .HasIndex(c => c.Title);

            modelBuilder.Entity<ContactMessage>()
                .HasIndex(cm => cm.Email)
                .IsUnique();

            modelBuilder.Entity<Capsule>()
                .HasOne(c => c.CreatedByUser) 
                .WithMany() 
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CapsuleRecipient>()
                .HasOne(cr => cr.Capsule)       
                .WithMany(c => c.CapsuleRecipients)   
                .HasForeignKey(cr => cr.CapsuleId)
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<CapsuleAnswer>()
                .HasOne(ca => ca.Capsule)
                .WithMany(c => c.CapsuleAnswers)
                .HasForeignKey(ca => ca.CapsuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CapsuleAnswer>()
                .HasOne(ca => ca.CapsuleQuestion)
                .WithMany(q => q.CapsuleAnswers)
                .HasForeignKey(ca => ca.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);            
        }
    }
}
