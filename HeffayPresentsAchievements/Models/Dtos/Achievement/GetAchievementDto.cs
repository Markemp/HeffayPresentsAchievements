using HeffayPresentsAchievements.Models;
using System;

namespace HeffayPresentsAchievements.Dtos.Achievement
{
    public record GetAchievementDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Points { get; set; }
        public bool IsIncrementalAchievement { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public AchievementType AchievementType { get; set; }
        public Guid GameId { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Id: {Id}, Points: {Points}, Type: {AchievementType}";
        }
    }
}
