using Journal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journal.Services
{
    public class JournalService
    {
        private readonly List<JournalEntry> _entries = new();

        public JournalService()
        {
            _entries.Add(new JournalEntry
            {
                Date = DateTime.Today.AddDays(-1),
                Title = "Yesterday",
                Content = "Had a good day.",
                Category = "General",
                PrimaryMood = new Mood { Name = "Happy", Emoji = "😊", Category = MoodCategory.Positive },
                SecondaryMoods = new List<Mood>(),
                Tags = new List<string> { "Work", "Exercise" },
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            });
        }

        public List<Mood> GetAvailableMoods()
        {
            return PredefinedMoods.All;
        }

        public List<string> GetAvailableTags()
        {
            return PredefinedTags.All.ToList();
        }

        public JournalEntry? GetEntryByDate(DateTime date)
        {
            return _entries.FirstOrDefault(e => e.Date.Date == date.Date);
        }

        public List<JournalEntry> GetAllEntries()
        {
            return _entries.OrderByDescending(e => e.Date).ToList();
        }

        public List<JournalEntry> SearchEntries(
            string? query,
            DateTime? from,
            DateTime? to,
            List<string>? moods,
            List<string>? tags)
        {
            IEnumerable<JournalEntry> result = _entries;

            if (from.HasValue)
                result = result.Where(e => e.Date.Date >= from.Value.Date);

            if (to.HasValue)
                result = result.Where(e => e.Date.Date <= to.Value.Date);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                result = result.Where(e =>
                    (e.Title?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Content?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (moods != null && moods.Count > 0)
            {
                result = result.Where(e =>
                    (e.PrimaryMood != null && moods.Contains(e.PrimaryMood.Name)) ||
                    (e.SecondaryMoods != null && e.SecondaryMoods.Any(sm => moods.Contains(sm.Name))));
            }

            if (tags != null && tags.Count > 0)
            {
                result = result.Where(e => e.Tags != null && e.Tags.Any(t => tags.Contains(t)));
            }

            return result.OrderByDescending(e => e.Date).ToList();
        }

        public void SaveEntry(JournalEntry entry)
        {
            ValidateEntry(entry);

            var existingByDate = _entries.FirstOrDefault(e => e.Date.Date == entry.Date.Date);
            if (existingByDate != null)
            {
                existingByDate.Title = entry.Title;
                existingByDate.Content = entry.Content;
                existingByDate.Category = entry.Category;
                existingByDate.PrimaryMood = entry.PrimaryMood;
                existingByDate.SecondaryMoods = entry.SecondaryMoods;
                existingByDate.Tags = entry.Tags;
                existingByDate.UpdatedAt = DateTime.Now;
                return;
            }

            entry.CreatedAt = DateTime.Now;
            entry.UpdatedAt = DateTime.Now;
            _entries.Add(entry);
        }

        public void DeleteEntry(Guid id)
        {
            var entry = _entries.FirstOrDefault(e => e.Id == id);
            if (entry != null)
                _entries.Remove(entry);
        }

        public void DeleteEntryByDate(DateTime date)
        {
            var entry = _entries.FirstOrDefault(e => e.Date.Date == date.Date);
            if (entry != null)
                _entries.Remove(entry);
        }

        public JournalStats GetStats()
        {
            var stats = new JournalStats { TotalEntries = _entries.Count };

            if (!_entries.Any())
                return stats;

            var moodCounts = _entries
                .Where(e => e.PrimaryMood != null && !string.IsNullOrWhiteSpace(e.PrimaryMood.Name))
                .GroupBy(e => e.PrimaryMood.Name)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.MoodDistribution = moodCounts;
            if (moodCounts.Any())
                stats.MostFrequentMood = moodCounts.OrderByDescending(x => x.Value).First().Key;

            var tagCounts = _entries
                .SelectMany(e => e.Tags ?? new List<string>())
                .GroupBy(t => t)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.MostUsedTags = tagCounts;

            var streak = GetStreakStats();
            stats.CurrentStreak = streak.currentStreak;
            stats.LongestStreak = streak.longestStreak;
            stats.MissedDays = streak.missedDays;

            return stats;
        }

        public (int currentStreak, int longestStreak, int missedDays) GetStreakStats()
        {
            var dates = _entries
                .Select(e => e.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (dates.Count == 0)
                return (0, 0, 0);

            var first = dates.First();
            var today = DateTime.Today;

            int totalDays = (today - first).Days + 1;
            int missedDays = totalDays - dates.Count;

            int longest = 1, run = 1;
            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
                    run++;
                else
                    run = 1;

                if (run > longest)
                    longest = run;
            }

            int currentStreak = 0;
            var set = dates.ToHashSet();
            var day = today;
            while (set.Contains(day))
            {
                currentStreak++;
                day = day.AddDays(-1);
            }

            return (currentStreak, longest, Math.Max(0, missedDays));
        }

        private void ValidateEntry(JournalEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Title))
                throw new ArgumentException("Title is required.");

            if (entry.PrimaryMood == null || string.IsNullOrWhiteSpace(entry.PrimaryMood.Name))
                throw new ArgumentException("Primary mood is required.");

            if (entry.SecondaryMoods != null && entry.SecondaryMoods.Count > 2)
                throw new ArgumentException("You can select up to two secondary moods.");

            if (entry.SecondaryMoods != null &&
                entry.PrimaryMood != null &&
                entry.SecondaryMoods.Any(m => m.Name == entry.PrimaryMood.Name))
                throw new ArgumentException("Secondary moods cannot include the primary mood.");
        }
    }
}
