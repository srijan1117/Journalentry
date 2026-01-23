using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Data
{
    public class JournalEntryRecord
    {
        public Guid Id { get; set; }

        // Enforce "one entry per day" via unique index on Date
        public DateTime Date { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Store primary mood as simple fields
        public string PrimaryMoodName { get; set; } = string.Empty;
        public string PrimaryMoodEmoji { get; set; } = string.Empty;
        public int PrimaryMoodCategory { get; set; } // enum stored as int

        // Store lists as JSON for SQLite simplicity
        public string SecondaryMoodsJson { get; set; } = "[]";
        public string TagsJson { get; set; } = "[]";
    }
}
