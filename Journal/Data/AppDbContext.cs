using Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Journal.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<JournalEntryRecord> JournalEntries => Set<JournalEntryRecord>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var e = modelBuilder.Entity<JournalEntryRecord>();

            e.HasKey(x => x.Id);

            // Store only date portion by always saving Date.Date in service
            e.HasIndex(x => x.Date).IsUnique(); // ONE ENTRY PER DAY (DB-level guarantee)

            e.Property(x => x.Title).IsRequired();
            e.Property(x => x.Content).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
