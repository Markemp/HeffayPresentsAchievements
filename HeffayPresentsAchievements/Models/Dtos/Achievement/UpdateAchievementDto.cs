using HeffayPresentsAchievements.Models;
using System;

namespace HeffayPresentsAchievements.Dtos.Achievement
{
    public record UpdateAchievementDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? Points { get; set; }
        public float? PercentageUnlocked { get; set; }
        public bool? IsIncrementalAchievement { get; set; }
        public bool? IsDeleted { get; set; }
        public AchievementType? AchievementType { get; set; }
    }
}
