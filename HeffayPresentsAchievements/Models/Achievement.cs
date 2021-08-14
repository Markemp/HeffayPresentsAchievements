using System;

namespace HeffayPresentsAchievements.Models
{
    public class Achievement : BaseEntity
    {
        public string Name { get; set; } = "First Achievement";
        public int Points { get; set; } = 10;
        public float PercentageUnlocked { get; set; } = 0.0f;
        public bool IsIncrementalAchievement { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public AchievementType AchievementType { get; set; } = AchievementType.Visible;
        public virtual Game? Game { get; set; }
    }
}
