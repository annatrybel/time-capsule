using Microsoft.EntityFrameworkCore;

namespace TimeCapsule.Models
{
    public class TimeCapsuleContext : DbContext
    {
        public TimeCapsuleContext(DbContextOptions options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
