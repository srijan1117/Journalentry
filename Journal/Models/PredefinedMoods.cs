using System.Collections.Generic;

namespace Journal.Models
{
    public static class PredefinedMoods
    {
        public static readonly List<Mood> All = new()
        {
            // Positive
            new Mood { Name = "Happy", Emoji = "😊", Category = MoodCategory.Positive },
            new Mood { Name = "Relaxed", Emoji = "😌", Category = MoodCategory.Positive },

            // Neutral
            new Mood { Name = "Calm", Emoji = "😐", Category = MoodCategory.Neutral },

            // Negative
            new Mood { Name = "Sad", Emoji = "😢", Category = MoodCategory.Negative },
            new Mood { Name = "Stressed", Emoji = "😫", Category = MoodCategory.Negative },
        };
    }
}
