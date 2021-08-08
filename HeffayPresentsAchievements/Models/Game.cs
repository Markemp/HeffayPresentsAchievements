using System;
using System.Collections.Generic;

namespace HeffayPresentsAchievements.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<Achievement>? Achievements { get; set; }
    }
}
