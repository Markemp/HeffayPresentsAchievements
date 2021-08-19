using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public class AchievementService : IAchievementService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Achievement> achievementRepo;
        private readonly IRepository<Game> gameRepo;
        private readonly IHttpContextAccessor _httpContext;

        public AchievementService(IMapper mapper, 
            IRepository<Achievement> achievementRepository, 
            IRepository<Game> gameRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            this.mapper = mapper;
            achievementRepo = achievementRepository;
            gameRepo = gameRepository; 
            _httpContext = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();

            try
            {
                var dbAchievements = await achievementRepo.GetAll();

                if (dbAchievements.Any())
                    response.Data = dbAchievements
                        .Where(a => a != null && a.IsDeleted == false)
                        .Select(a => mapper.Map<GetAchievementDto>(a))
                        .ToList();
                else
                    response.Message = "No achievements found.";

                return response;
            } 
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> GetAchievementById(Guid id)
        {
            var response = new ServiceResponse<GetAchievementDto>();

            try
            {
                var achievement = await achievementRepo.Get(id);
                if (achievement == null)
                {
                    response.Message = $"Achievement {id} not found.";
                    response.Success = false;
                }
                else
                {
                    response.Data = mapper.Map<GetAchievementDto>(await achievementRepo.Get(id));
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

        public async Task<ServiceResponse<GetAchievementDto>> AddAchievement(AddAchievementDto newAchievement)
        {
            var response = new ServiceResponse<GetAchievementDto>();

            try
            {
                Achievement achievement = mapper.Map<Achievement>(newAchievement);
                achievement.Id = Guid.NewGuid();
                achievement.LastUpdated = DateTime.UtcNow;
                achievement.Game = await gameRepo.Get(newAchievement.GameId);
                var rowsChanged = await achievementRepo.Add(achievement);
                var newAch = await achievementRepo.Get(achievement.Id);
                response.Data = mapper.Map<GetAchievementDto>(newAch);
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

        public async Task<ServiceResponse<GetAchievementDto>> UpdateAchievement(UpdateAchievementDto updatedAchievement)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            try
            {
                Achievement? achievement = await achievementRepo.Get(updatedAchievement.Id);

                if (achievement != null)
                {
                    // TODO:  Fix nullable stuff here
                    achievement.Name = updatedAchievement.Name ?? achievement.Name;
                    achievement.IsIncrementalAchievement = updatedAchievement.IsIncrementalAchievement;
                    achievement.IsDeleted = updatedAchievement.IsDeleted;
                    achievement.Points = updatedAchievement.Points;
                    achievement.AchievementType = updatedAchievement.AchievementType ?? achievement.AchievementType;
                    achievement.LastUpdated = DateTime.UtcNow;
                    await achievementRepo.Update(achievement);

                    response.Data = mapper.Map<GetAchievementDto>(achievement);
                }
                else
                {
                    response.Success = false;
                    response.Message = $"Achievement {updatedAchievement.Id} not found.";
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> DeleteAchievement(Guid id)
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();
            try
            {
                var rowsAffected = await achievementRepo.Remove(id);
                response.Message = $"Removed {rowsAffected} rows.";
                
                if (rowsAffected != 0)
                {
                    var dbAchievements = await achievementRepo.GetAll();
                    response.Data = dbAchievements.Where(a => a != null && a.IsDeleted == false).Select(a => mapper.Map<GetAchievementDto>(a)).ToList();
                }
                else
                {
                    response.Message = "Achievement not found.";
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = (ex.Message);
                response.Data = null;
            }

            return response;
        }
    }
}
