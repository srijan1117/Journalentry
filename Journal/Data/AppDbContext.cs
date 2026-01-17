using Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Journal.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
