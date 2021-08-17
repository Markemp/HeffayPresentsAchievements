using System;

namespace HeffayPresentsAchievements.Models.Dtos.Game
{
    public record GetGameDto
        (
            Guid GameId,
            string Name
        );
}
