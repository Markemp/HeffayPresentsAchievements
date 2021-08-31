using AutoMapper;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Achievement> _achievementRepo;
        private readonly IRepository<Game> _gameRepo;
        private readonly IHttpContextAccessor _httpContext;

        public GameService(IMapper mapper,
            IRepository<Achievement> achievementRepository,
            IRepository<Game> gameRepository,
            IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _achievementRepo = achievementRepository;
            _gameRepo = gameRepository;
            _httpContext = httpContext;
        }

        private Guid GetUserId()
        {
            if (_httpContext.HttpContext != null)
                return Guid.Parse(_httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            else
                throw new ApplicationException("Unable to find HttpContext for request.");
        }

        public async Task<ServiceResponse<List<GetGameDto>>> GetAllGames()
        {
            var response = new ServiceResponse<List<GetGameDto>>();
            
            try
            {
                var allGames = await _gameRepo.GetAllForId(GetUserId());
                if (allGames.Any())
                {
                    response.Data = allGames
                        .Where(g => g != null && g.IsDeleted == false)
                        .Select(g => _mapper.Map<GetGameDto>(g))
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

        public async Task<ServiceResponse<GetGameDto>> GetGameById(Guid gameId)
        {
            var response = new ServiceResponse<GetGameDto>();

            try
            {
                var game = await _gameRepo.Get(gameId);
                if (game == null)
                {
                    response.Message = $"Game {gameId} not found.";
                    response.Success = false;
                }
                else
                {
                    if (game.Users != null && game.Users.Any(u => u.Id == GetUserId()))
                    {
                        response.Data = _mapper.Map<GetGameDto>(await _gameRepo.Get(gameId));
                        response.Success = true;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = $"Game {gameId} not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = null;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<GetGameDto>> AddGame(AddGameDto newGameDto)
        {
            var response = new ServiceResponse<GetGameDto>();

            try
            {
                Game game = _mapper.Map<Game>(newGameDto);

                game.Id = Guid.NewGuid();
                game.LastUpdated = DateTime.UtcNow;
                //game.Users.Add(userRepository.GetById(GetUserId()));
                
                var rowsChanged = await _gameRepo.Add(game);
                var newGame = await _gameRepo.Get(game.Id);
                response.Data = _mapper.Map<GetGameDto>(newGame);
                response.Message = $"Added {rowsChanged} row (should be 1).";
            }
            catch (ArgumentNullException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetGameDto>>> DeleteGame(Guid id)
        {
            var response = new ServiceResponse<List<GetGameDto>>();

            try
            {
                var rowsAffected = await _gameRepo.Remove(id);
                response.Message = $"Removed {rowsAffected} rows.";

                if (rowsAffected != 0)
                {
                    var allGames = await _gameRepo.GetAll();
                    response.Data = allGames.Where(a => a != null && a.IsDeleted == false).Select(a => _mapper.Map<GetGameDto>(a)).ToList();
                }
                else
                {
                    response.Message = "Game not found.";
                    response.Success = false;
                }
            }   
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }

        public async Task<ServiceResponse<GetGameDto>> UpdateGame(UpdateGameDto updateGame)
        {
            var response = new ServiceResponse<GetGameDto>();

            try
            {
                var game = await _gameRepo.Get(updateGame.Id);

                if (game != null)
                {
                    game.Name = updateGame.Name;
                    await _gameRepo.Update(game);

                    response.Data = _mapper.Map<GetGameDto>(game);
                }
                else
                {
                    response.Success = false;
                    response.Message = $"Game {updateGame.Name} not found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
