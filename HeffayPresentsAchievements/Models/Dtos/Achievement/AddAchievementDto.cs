﻿using HeffayPresentsAchievements.Models;
using System;

namespace HeffayPresentsAchievements.Dtos.Achievement
{
    public class AddAchievementDto
    {
        public string? Name { get; set; }
        public int Points { get; set; }
        public bool IsIncrementalAchievement { get; set; }
        public bool IsDeleted { get; set; }
        public AchievementType AchievementType { get; set; }
        public Guid GameId { get; set; }
    }
}
