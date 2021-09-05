using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public class AchievementService : IAchievementService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Achievement> _achievementRepo;

        public AchievementService(IMapper mapper, 
            IRepository<Achievement> achievementRepository)
        {
            _mapper = mapper;
            _achievementRepo = achievementRepository;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();

            try
            {
                var dbAchievements = await _achievementRepo.GetAll();

                if (dbAchievements.Any())
                    response.Data = dbAchievements
                        .Where(a => a != null && a.IsDeleted == false)
                        .Select(a => _mapper.Map<GetAchievementDto>(a))
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
                var achievement = await _achievementRepo.Get(id);
                if (achievement == null)
                {
                    response.Message = $"Achievement {id} not found.";
                    response.Success = false;
                }
                else
                {
                    response.Data = _mapper.Map<GetAchievementDto>(await _achievementRepo.Get(id));
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
                Achievement achievement = _mapper.Map<Achievement>(newAchievement);
                achievement.Id = Guid.NewGuid();
                achievement.LastUpdated = DateTime.UtcNow;
                var rowsChanged = await _achievementRepo.Add(achievement);
                var newAch = await _achievementRepo.Get(achievement.Id);
                response.Data = _mapper.Map<GetAchievementDto>(newAch);
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
                Achievement? achievement = await _achievementRepo.Get(updatedAchievement.Id);

                if (achievement != null)
                {
                    // TODO:  Fix nullable stuff here
                    achievement.Name = updatedAchievement.Name ?? achievement.Name;
                    achievement.IsIncrementalAchievement = updatedAchievement.IsIncrementalAchievement;
                    achievement.IsDeleted = updatedAchievement.IsDeleted;
                    achievement.Points = updatedAchievement.Points;
                    achievement.AchievementType = updatedAchievement.AchievementType ?? achievement.AchievementType;
                    achievement.LastUpdated = DateTime.UtcNow;
                    await _achievementRepo.Update(achievement);

                    response.Data = _mapper.Map<GetAchievementDto>(achievement);
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
                var rowsAffected = await _achievementRepo.Remove(id);
                response.Message = $"Removed {rowsAffected} rows.";
                
                if (rowsAffected != 0)
                {
                    var dbAchievements = await _achievementRepo.GetAll();
                    response.Data = dbAchievements.Where(a => a != null && a.IsDeleted == false).Select(a => _mapper.Map<GetAchievementDto>(a)).ToList();
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
