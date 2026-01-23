using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Journal.Data;
using Journal.Models;

namespace Journal.Services
{
    public static class EntryMapper
    {
        private record MoodDto(string Name, string Emoji, int Category);

        public static JournalEntry ToModel(JournalEntryRecord r)
        {
            var secondaryDtos = JsonSerializer.Deserialize<List<MoodDto>>(r.SecondaryMoodsJson) ?? new();
            var tags = JsonSerializer.Deserialize<List<string>>(r.TagsJson) ?? new();

            return new JournalEntry
            {
                Id = r.Id,
                Date = r.Date.Date,
                Title = r.Title,
                Content = r.Content,
                Category = r.Category,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                PrimaryMood = new Mood
                {
                    Name = r.PrimaryMoodName,
                    Emoji = r.PrimaryMoodEmoji,
                    Category = (MoodCategory)r.PrimaryMoodCategory
                },
                SecondaryMoods = secondaryDtos.Select(d => new Mood
                {
                    Name = d.Name,
                    Emoji = d.Emoji,
                    Category = (MoodCategory)d.Category
                }).ToList(),
                Tags = tags
            };
        }

        public static JournalEntryRecord ToRecord(JournalEntry m)
        {
            var secondaryDtos = (m.SecondaryMoods ?? new List<Mood>())
                .Select(x => new MoodDto(x.Name, x.Emoji, (int)x.Category))
                .ToList();

            var tags = m.Tags ?? new List<string>();

            return new JournalEntryRecord
            {
                Id = m.Id == Guid.Empty ? Guid.NewGuid() : m.Id,
                Date = m.Date.Date,
                Title = m.Title,
                Content = m.Content,
                Category = m.Category,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                PrimaryMoodName = m.PrimaryMood?.Name ?? "",
                PrimaryMoodEmoji = m.PrimaryMood?.Emoji ?? "",
                PrimaryMoodCategory = (int)(m.PrimaryMood?.Category ?? MoodCategory.Neutral),
                SecondaryMoodsJson = JsonSerializer.Serialize(secondaryDtos),
                TagsJson = JsonSerializer.Serialize(tags)
            };
        }
    }
}
