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
        public DbSet<CapsuleSection> CapsuleSections { get; set; }
        public DbSet<CapsuleLink> CapsuleLinks { get; set; }
        public DbSet<CapsuleRecipient> CapsuleRecipients { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDto>().HasNoKey().ToView("UserWithRoles");

            modelBuilder.Entity<Capsule>(entity =>
            {
                entity.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);  //nie usuwamy użytkownika, jeśli ma stworzone kapsuły.

                entity.HasIndex(c => c.OpeningDate);
                entity.HasIndex(c => c.Status);
                entity.HasIndex(c => c.Title);       	 
            });

            modelBuilder.Entity<CapsuleRecipient>(entity =>
            {
                entity.HasOne(cr => cr.Capsule)
                .WithMany(c => c.CapsuleRecipients)
                .HasForeignKey(cr => cr.CapsuleId)
                .OnDelete(DeleteBehavior.Cascade); //usunięcie kapsuły powoduje usunięcie jej odbiorców

                entity.HasIndex(cr => new { cr.CapsuleId, cr.Email })
                      .IsUnique();              //unikalny email w ramach jednej kapsuły.
            });

            modelBuilder.Entity<CapsuleAnswer>(entity =>
            {
                entity.HasOne(ca => ca.Capsule)
                      .WithMany(c => c.CapsuleAnswers)
                      .HasForeignKey(ca => ca.CapsuleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(ca => ca.CapsuleQuestion)
                      .WithMany(q => q.CapsuleAnswers)
                      .HasForeignKey(ca => ca.QuestionId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<CapsuleImage>(entity =>
            {
                entity.HasOne(ci => ci.Capsule)
                      .WithMany(c => c.CapsuleImages)
                      .HasForeignKey(ci => ci.CapsuleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<CapsuleLink>(entity =>
            {
                entity.HasOne(cl => cl.Capsule)
                      .WithMany(c => c.CapsuleLinks)
                      .HasForeignKey(cl => cl.CapsuleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<CapsuleSection>(entity =>
            {
                entity.HasIndex(cs => new { cs.Name, cs.CapsuleType })   //  Nazwa sekcji unikalna dla typu kapsuły
                    .IsUnique();

                entity.HasIndex(cs => cs.DisplayOrder); // Indeks dla kolejności wyświetlania
            });

            modelBuilder.Entity<CapsuleQuestion>(entity =>
            {
                entity.HasOne(cq => cq.CapsuleSection)
                      .WithMany(cs => cs.Questions)
                      .HasForeignKey(cq => cq.CapsuleSectionId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasIndex(cq => cq.DisplayOrder);   
            });
        }
    }
}


