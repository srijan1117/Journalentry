using System.Collections.Generic;

namespace Journal.Models
{
    public class JournalStats
    {
        public int TotalEntries { get; set; }

        // Primary mood counts by mood name
        public Dictionary<string, int> MoodDistribution { get; set; } = new();

        public string MostFrequentMood { get; set; } = string.Empty;

        // Tag counts by tag name
        public Dictionary<string, int> MostUsedTags { get; set; } = new();

        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int MissedDays { get; set; }
    }
}
