using System;
using System.Collections.Generic;

namespace Journal.Models
{
    public class JournalEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Mood? PrimaryMood { get; set; }
        public List<Mood> SecondaryMoods { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}
