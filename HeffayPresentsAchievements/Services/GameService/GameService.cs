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
        private readonly IMapper _mapper;
        private readonly IRepository<Achievement> _achievemenetRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IHttpContextAccessor _httpContext;

        public GameService(IMapper mapper,
            IRepository<Achievement> achievementRepo,
            IRepository<Game> gameRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _achievemenetRepository = achievementRepo;
            _gameRepository = gameRepo;
            _httpContext = httpContextAccessor;
        }

        public Task<ServiceResponse<List<GetGameDto>>> GetGames()
        {
            throw new NotImplementedException();
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
