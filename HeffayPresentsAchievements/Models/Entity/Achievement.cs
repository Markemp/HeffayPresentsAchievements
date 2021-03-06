using System;

namespace HeffayPresentsAchievements.Models
{
    public class Achievement : BaseEntity
    {
        public string? Name { get; set; }
        public int Points { get; set; }
        public bool IsIncrementalAchievement { get; set; }
        public bool IsDeleted { get; set; }
        public AchievementType AchievementType { get; set; }
        public Guid? GameId { get; set; }
    }
}
