using System.Collections.Generic;

namespace Journal.Models
{
    public class JournalStats
    {
        public int TotalEntries { get; set; }
        public int CurrentStreak { get; set; }
        public string MostFrequentMood { get; set; } = "N/A";
        public int MissedDays { get; set; }
        public Dictionary<string, int> MoodDistribution { get; set; } = new();
        public Dictionary<string, int> MostUsedTags { get; set; } = new();
    }
}
