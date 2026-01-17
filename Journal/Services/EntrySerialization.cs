using System.Collections.Generic;
using System.Text.Json;

namespace Journal.Services
{
    public static class EntrySerialization
    {
        public static List<string> FromJson(string json)
            => JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

        public static string ToJson(List<string> items)
            => JsonSerializer.Serialize(items ?? new List<string>());
    }
}
