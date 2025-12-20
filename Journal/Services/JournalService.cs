using Journal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journal.Services
{
    public class JournalService
    {
        private List<JournalEntry> _entries = new();

        public JournalService()
        {
            // Seed some data for testing
            _entries.Add(new JournalEntry
            {
                Date = DateTime.Today.AddDays(-1),
                Title = "Yesterday",
                Content = "Had a good day.",
                PrimaryMood = new Mood { Name = "Happy", Emoji = "😊" },
                Tags = new List<string> { "Work", "Exercise" }
            });
        }

        public List<Mood> GetAvailableMoods()
        {
            return new List<Mood>
            {
                new Mood { Name = "Happy", Emoji = "😊" },
                new Mood { Name = "Sad", Emoji = "😢" },
                new Mood { Name = "Excited", Emoji = "🤩" },
                new Mood { Name = "Tired", Emoji = "😴" },
                new Mood { Name = "Angry", Emoji = "😡" },
                new Mood { Name = "Neutral", Emoji = "😐" }
            };
        }

        public List<string> GetAvailableTags()
        {
            return new List<string> { "Work", "Family", "Health", "Travel", "Hobby", "Social" };
        }

        public JournalEntry? GetEntryByDate(DateTime date)
        {
            return _entries.FirstOrDefault(e => e.Date.Date == date.Date);
        }

        public List<JournalEntry> GetAllEntries()
        {
            return _entries.OrderByDescending(e => e.Date).ToList();
        }

        public void SaveEntry(JournalEntry entry)
        {
            var existing = _entries.FirstOrDefault(e => e.Id == entry.Id);
            if (existing != null)
            {
                existing.Title = entry.Title;
                existing.Content = entry.Content;
                existing.PrimaryMood = entry.PrimaryMood;
                existing.SecondaryMoods = entry.SecondaryMoods;
                existing.Tags = entry.Tags;
                existing.UpdatedAt = DateTime.Now;
            }
            else
            {
                // Check if entry already exists for this date
                var duplicateDate = _entries.FirstOrDefault(e => e.Date.Date == entry.Date.Date);
                if (duplicateDate != null)
                {
                    // Update the existing one instead of adding new
                    duplicateDate.Title = entry.Title;
                    duplicateDate.Content = entry.Content;
                    duplicateDate.PrimaryMood = entry.PrimaryMood;
                    duplicateDate.SecondaryMoods = entry.SecondaryMoods;
                    duplicateDate.Tags = entry.Tags;
                    duplicateDate.UpdatedAt = DateTime.Now;
                    return;
                }

                entry.CreatedAt = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                if (entry.Id == Guid.Empty) entry.Id = Guid.NewGuid();
                _entries.Add(entry);
            }
        }

        public void DeleteEntry(Guid id)
        {
            var entry = _entries.FirstOrDefault(e => e.Id == id);
            if (entry != null)
            {
                _entries.Remove(entry);
            }
        }

        public JournalStats GetStats()
        {
            var stats = new JournalStats
            {
                TotalEntries = _entries.Count
            };

            if (_entries.Any())
            {
                // Most Frequent Mood
                var moodCounts = _entries
                    .Where(e => e.PrimaryMood != null)
                    .GroupBy(e => e.PrimaryMood!.Name)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                stats.MoodDistribution = moodCounts;
                if (moodCounts.Any())
                {
                    stats.MostFrequentMood = moodCounts.OrderByDescending(x => x.Value).First().Key;
                }

                // Most Used Tags
                var tagCounts = _entries
                    .SelectMany(e => e.Tags)
                    .GroupBy(t => t)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                stats.MostUsedTags = tagCounts;

                // Streak (simplified)
                stats.CurrentStreak = CalculateStreak();
                stats.MissedDays = 0; // Simplified
            }

            return stats;
        }

        private int CalculateStreak()
        {
            int streak = 0;
            var dates = _entries.Select(e => e.Date.Date).OrderByDescending(d => d).Distinct().ToList();
            var current = DateTime.Today;

            if (dates.Contains(current))
            {
                streak++;
                current = current.AddDays(-1);
            }
            else if (dates.Contains(current.AddDays(-1)))
            {
                current = current.AddDays(-1);
            }
            else
            {
                return 0;
            }

            while (dates.Contains(current))
            {
                streak++;
                current = current.AddDays(-1);
            }

            return streak;
        }
    }
}
