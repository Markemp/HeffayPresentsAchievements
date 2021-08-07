using HeffayPresentsAchievements.Models;
using System;

namespace HeffayPresentsAchievements.Dtos.Achievement
{
    public class GetAchievementDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = "First Achievement";
        public int Points { get; set; } = 10;
        public float PercentageUnlocked { get; set; } = 0.0f;
        public bool IsIncrementalAchievement { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public AchievementType AchievementType { get; set; } = AchievementType.Visible;
    }
}
