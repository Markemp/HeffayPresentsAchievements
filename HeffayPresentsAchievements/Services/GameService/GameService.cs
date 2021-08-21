using AutoMapper;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Achievement> achievementRepo;
        private readonly IRepository<Game> gameRepo;

        public GameService(IMapper mapper,
            IRepository<Achievement> achievementRepository,
            IRepository<Game> gameRepository)
        {
            this.mapper = mapper;
            achievementRepo = achievementRepository;
            gameRepo = gameRepository;
        }

        public async Task<ServiceResponse<List<GetGameDto>>> GetAllGames()
        {
            var response = new ServiceResponse<List<GetGameDto>>();
            
            try
            {
                var allGames = await gameRepo.GetAll();
                if (allGames.Any())
                {
                    response.Data = allGames
                        .Where(g => g != null && g.IsDeleted == false)
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

        public async Task<ServiceResponse<GetGameDto>> GetGameById(Guid id)
        {
            var response = new ServiceResponse<GetGameDto>();

            try
            {
                var achievement = await gameRepo.Get(id);
                if (achievement == null)
                {
                    response.Message = $"Game {id} not found.";
                    response.Success = false;
                }
                else
                {
                    response.Data = mapper.Map<GetGameDto>(await gameRepo.Get(id));
                    response.Success = true;
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
                Game game = mapper.Map<Game>(newGameDto);
                game.Id = Guid.NewGuid();
                game.LastUpdated = DateTime.UtcNow;
                var rowsChanged = await gameRepo.Add(game);
                var newGame = await gameRepo.Get(game.Id);
                response.Data = mapper.Map<GetGameDto>(newGame);
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
                var rowsAffected = await gameRepo.Remove(id);
                response.Message = $"Removed {rowsAffected} rows.";

                if (rowsAffected != 0)
                {
                    var allGames = await gameRepo.GetAll();
                    response.Data = allGames.Where(a => a != null && a.IsDeleted == false).Select(a => mapper.Map<GetGameDto>(a)).ToList();
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
                var game = await gameRepo.Get(updateGame.Id);

                if (game != null)
                {
                    game.Name = updateGame.Name;
                    await gameRepo.Update(game);

                    response.Data = mapper.Map<GetGameDto>(game);
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
