using System.Collections.Generic;

namespace Journal.Models
{
    public static class PredefinedMoods
    {
        public static readonly List<Mood> All = new()
        {
            // Positive
            new Mood { Name="Happy", Emoji="😊", Category=MoodCategory.Positive },
            new Mood { Name="Excited", Emoji="🤩", Category=MoodCategory.Positive },
            new Mood { Name="Relaxed", Emoji="😌", Category=MoodCategory.Positive },
            new Mood { Name="Grateful", Emoji="🙏", Category=MoodCategory.Positive },
            new Mood { Name="Confident", Emoji="💪", Category=MoodCategory.Positive },

            // Neutral
            new Mood { Name="Calm", Emoji="🙂", Category=MoodCategory.Neutral },
            new Mood { Name="Thoughtful", Emoji="🤔", Category=MoodCategory.Neutral },
            new Mood { Name="Curious", Emoji="🧐", Category=MoodCategory.Neutral },
            new Mood { Name="Nostalgic", Emoji="🕰️", Category=MoodCategory.Neutral },
            new Mood { Name="Bored", Emoji="😐", Category=MoodCategory.Neutral },

            // Negative
            new Mood { Name="Sad", Emoji="😢", Category=MoodCategory.Negative },
            new Mood { Name="Angry", Emoji="😠", Category=MoodCategory.Negative },
            new Mood { Name="Stressed", Emoji="😫", Category=MoodCategory.Negative },
            new Mood { Name="Lonely", Emoji="🥺", Category=MoodCategory.Negative },
            new Mood { Name="Anxious", Emoji="😰", Category=MoodCategory.Negative },
        };
    }
}
