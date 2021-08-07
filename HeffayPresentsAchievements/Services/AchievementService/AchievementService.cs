using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public class AchievementService : IAchievementService
    {
        private static readonly List<Achievement> achievements = new()
        {
            new Achievement { Id = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"), Name = "First achievement" },
            new Achievement { Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e340"), Name = "Second achievement" }
        };
        private readonly IMapper _mapper;

        public AchievementService(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        public async Task<ServiceResponse<List<GetAchievementDto>>> AddAchievement(AddAchievementDto newAchievement)
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();
            Achievement achievement = _mapper.Map<Achievement>(newAchievement);
            achievement.Id = Guid.NewGuid();
            achievement.LastUpdated = DateTime.UtcNow;
            achievements.Add(achievement);
            response.Data = achievements.Select(a => _mapper.Map<GetAchievementDto>(a)).ToList();
            return response;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var response = new ServiceResponse<List<GetAchievementDto>>
            {
                Data = achievements.Where(a => a.IsDeleted == false).Select(a => _mapper.Map<GetAchievementDto>(a)).ToList()
            };
            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> GetAchievementById(Guid id)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            var achievement = _mapper.Map<GetAchievementDto>(achievements.FirstOrDefault(a => a.Id.Equals(id)));

            if (achievement == null)
            {
                response.Success = false;
                response.Message = $"Unable to find achievement {id}";
            }
            else
            {
                response.Data = achievement;
            }

            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> UpdateAchievement(UpdateAchievementDto updatedAchievement)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            try
            {
                Achievement? achievement = achievements.FirstOrDefault(a => a.Id.Equals(updatedAchievement.Id));

                if (achievement != null)
                {
                    achievement.Name = updatedAchievement.Name ?? achievement.Name;
                    achievement.IsIncrementalAchievement = updatedAchievement.IsIncrementalAchievement ?? achievement.IsIncrementalAchievement;
                    achievement.PercentageUnlocked = updatedAchievement.PercentageUnlocked ?? achievement.PercentageUnlocked;
                    achievement.Points = updatedAchievement.Points ?? achievement.Points;
                    achievement.AchievementType = updatedAchievement.AchievementType ?? achievement.AchievementType;
                    achievement.LastUpdated = DateTime.UtcNow;

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
                var numberOfRecordsRemoved = achievements.RemoveAll(a => a.Id.Equals(id));
                response.Message = $"Removed {numberOfRecordsRemoved} achievements.";
                response.Data = _mapper.Map<List<GetAchievementDto>>(achievements.Where(a => a.IsDeleted == false));
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = (ex.Message);
            }

            return response;
        }
    }
}