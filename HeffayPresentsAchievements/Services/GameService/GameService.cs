using AutoMapper;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Achievement> _achievementRepo;
        private readonly IRepository<Game> _gameRepo;
        private readonly IHttpContextAccessor _httpContext;

        public GameService(IMapper mapper,
            IRepository<Achievement> achievementRepo,
            IRepository<Game> gameRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            this.mapper = mapper;
            _achievementRepo = achievementRepo;
            _gameRepo = gameRepo;
            _httpContext = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetGameDto>>> GetAllGames()
        {
            var response = new ServiceResponse<List<GetGameDto>>();
            
            try
            {
                var allGames = await _gameRepo.GetAll();
                if (allGames.Any())
                {
                    response.Data = allGames
                        .Where(g => g.IsDeleted == false)
                        .Select(g => mapper.Map<GetGameDto>(g))
                        .ToList();
                }
                else
                {
                    response.Success = true;
                    response.Message = "No games found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public Task<ServiceResponse<GetGameDto>> AddGame(AddGameDto newGame)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetGameDto>>> DeleteGame(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetGameDto>> GetGameById(Guid id)
        {
            throw new NotImplementedException();
        }


        public Task<ServiceResponse<GetGameDto>> UpdateGame(UpdateGameDto updateGame)
        {
            throw new NotImplementedException();
        }
    }
}
