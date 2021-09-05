using System;

namespace HeffayPresentsAchievements.Models.Dtos.UserGame
{
    public record AddUserGameDto(
        Guid GameId,
        Guid UserId
    );
}
