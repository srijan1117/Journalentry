using System;
using System.Collections.Generic;

namespace Journal.Models
{
    public class JournalEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // One entry per day
        public DateTime Date { get; set; } = DateTime.Today;

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // System timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Category { get; set; } = string.Empty;

        // Mood tracking
        public Mood PrimaryMood { get; set; } = new Mood();
        public List<Mood> SecondaryMoods { get; set; } = new();

        // Tags (prebuilt + custom)
        public List<string> Tags { get; set; } = new();
    }
}
