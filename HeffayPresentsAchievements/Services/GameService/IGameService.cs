using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.GameService
{
    public interface IGameService
    {
        Task<ServiceResponse<List<GetGameDto>>> GetGames();
        Task<ServiceResponse<GetGameDto>> GetGameById(Guid id);
        Task<ServiceResponse<GetGameDto>> AddGame(AddGameDto newGame);
        Task<ServiceResponse<GetGameDto>> UpdateGame(UpdateGameDto updateGame);
        Task<ServiceResponse<List<GetGameDto>>> DeleteGame(Guid id);
    }
}
