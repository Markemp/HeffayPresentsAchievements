using System;

namespace HeffayPresentsAchievements.Models.Dtos.Game
{
    public record AddGameDto(string Name, Guid UserId);
}
