using System.Text.Json.Serialization;

namespace HeffayPresentsAchievements.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AchievementType
    {
        Visible,
        Hidden
    }
}