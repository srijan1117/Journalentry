using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Journal.Data;
using Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Journal.Services
{
    public class JournalService
    {
        private readonly AppDbContext _db;

        public JournalService(AppDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated(); // creates DB/tables if missing
        }

        public List<Mood> GetAvailableMoods() => PredefinedMoods.All;
        public List<string> GetAvailableTags() => PredefinedTags.All.ToList();

        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            var d = date.Date;
            var record = await _db.JournalEntries.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Date == d);

            return record == null ? null : EntryMapper.ToModel(record);
        }

        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            var records = await _db.JournalEntries.AsNoTracking()
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return records.Select(EntryMapper.ToModel).ToList();
        }

        public async Task SaveEntryAsync(JournalEntry entry)
        {
            ValidateEntry(entry);

            entry.Date = entry.Date.Date;

            var existing = await _db.JournalEntries
                .FirstOrDefaultAsync(e => e.Date == entry.Date);

            if (existing != null)
            {
                existing.Title = entry.Title;
                existing.Content = entry.Content;
                existing.Category = entry.Category;

                existing.PrimaryMoodName = entry.PrimaryMood.Name;
                existing.PrimaryMoodEmoji = entry.PrimaryMood.Emoji;
                existing.PrimaryMoodCategory = (int)entry.PrimaryMood.Category;

                var updated = EntryMapper.ToRecord(entry);
                existing.SecondaryMoodsJson = updated.SecondaryMoodsJson;
                existing.TagsJson = updated.TagsJson;

                existing.UpdatedAt = DateTime.Now;

                await _db.SaveChangesAsync();
                return;
            }

            entry.CreatedAt = DateTime.Now;
            entry.UpdatedAt = DateTime.Now;

            var record = EntryMapper.ToRecord(entry);

            // DB-level unique Date index will enforce one entry/day even if logic breaks
            _db.JournalEntries.Add(record);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteEntryByDateAsync(DateTime date)
        {
            var d = date.Date;
            var existing = await _db.JournalEntries
                .FirstOrDefaultAsync(e => e.Date == d);

            if (existing == null) return;

            _db.JournalEntries.Remove(existing);
            await _db.SaveChangesAsync();
        }

        public async Task<List<JournalEntry>> SearchEntriesAsync(
            string? query,
            DateTime? from,
            DateTime? to,
            List<string>? moods,
            List<string>? tags)
        {
            var q = _db.JournalEntries.AsNoTracking().AsQueryable();

            if (from.HasValue) q = q.Where(e => e.Date >= from.Value.Date);
            if (to.HasValue) q = q.Where(e => e.Date <= to.Value.Date);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var s = query.Trim();
                q = q.Where(e =>
                    e.Title.Contains(s) ||
                    e.Content.Contains(s));
            }

            // Mood filter: primary mood is stored as columns (easy to filter)
            if (moods != null && moods.Count > 0)
            {
                q = q.Where(e => moods.Contains(e.PrimaryMoodName));
            }

            // Tags filter: stored as JSON -> filter in memory for now (simple)
            var records = await q.OrderByDescending(e => e.Date).ToListAsync();
            var models = records.Select(EntryMapper.ToModel).ToList();

            if (tags != null && tags.Count > 0)
            {
                models = models.Where(m => m.Tags.Any(t => tags.Contains(t))).ToList();
            }

            return models;
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

        public async Task<JournalStats> GetStatsAsync()
        {
            var dates = await _db.JournalEntries
                .AsNoTracking()
                .OrderByDescending(e => e.Date)
                .Select(e => e.Date)
                .ToListAsync();

            var stats = new JournalStats
            {
                TotalEntries = dates.Count
            };

            if (dates.Count == 0) return stats;

            // Ensure dates are unique and sorted (DB enforces unique, but good to be safe)
            var sortedDates = dates.Distinct().OrderByDescending(d => d).ToList();

            // --- Calculate Current Streak ---
            // A streak is kept alive if the last entry is Today OR Yesterday.
            // If the last entry is older than yesterday, the current streak is 0.
            
            int currentStreak = 0;
            var today = DateTime.Today;
            var lastEntryDate = sortedDates.First();

            if (lastEntryDate == today || lastEntryDate == today.AddDays(-1))
            {
                // Count backwards from the last entry
                currentStreak = 1;
                for (int i = 1; i < sortedDates.Count; i++)
                {
                    var expectedDate = lastEntryDate.AddDays(-i);
                    if (sortedDates[i] == expectedDate)
                    {
                        currentStreak++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            stats.CurrentStreak = currentStreak;

            // --- Calculate Longest Streak ---
            int longestStreak = 0;
            int tempStreak = 0;

            for (int i = 0; i < sortedDates.Count; i++)
            {
                if (i == 0)
                {
                    tempStreak = 1;
                }
                else
                {
                    var prevDate = sortedDates[i - 1];
                    var currDate = sortedDates[i];

                    if (currDate == prevDate.AddDays(-1))
                    {
                        tempStreak++;
                    }
                    else
                    {
                        tempStreak = 1;
                    }
                }

                if (tempStreak > longestStreak)
                {
                    longestStreak = tempStreak;
                }
            }
            stats.LongestStreak = longestStreak;

            return stats;
        }
    }
}
