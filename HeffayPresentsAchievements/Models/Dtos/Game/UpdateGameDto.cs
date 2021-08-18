using System;

namespace HeffayPresentsAchievements.Models.Dtos.Game
{
    public record UpdateGameDto
    (
        Guid Id,
        string Name
    );
}
