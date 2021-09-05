using HeffayPresentsAchievements.Models;
using System;

namespace HeffayPresentsAchievements.Dtos.Achievement
{
    public record AddAchievementDto
    (
        string? Name,
        int Points,
        bool IsIncrementalAchievement,
        bool IsDeleted,
        AchievementType AchievementType
    );
}
