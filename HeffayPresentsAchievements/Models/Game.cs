using System.Collections.Generic;

namespace HeffayPresentsAchievements.Models
{
    public class Game : BaseEntity
    {
        public string? Name { get; set; }
        public List<Achievement>? Achievements { get; set; }
    }
}
