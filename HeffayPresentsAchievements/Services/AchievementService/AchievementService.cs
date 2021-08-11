using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.EntityFrameworkCore;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public class AchievementService : IAchievementService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Achievement> _repository;

        public AchievementService(IMapper mapper, IRepository<Achievement> repo)
        {
            _mapper = mapper;
            _repository = repo;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var dbAchievements = await _repository.GetAll();
            var response = new ServiceResponse<List<GetAchievementDto>>
            {
                Data = dbAchievements.Where(a => a.IsDeleted == false).Select(a => _mapper.Map<GetAchievementDto>(a)).ToList()
            };
            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> GetAchievementById(Guid id)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            var achievement = await _repository.Get(id);

            if (achievement == null)
            {
                response.Success = false;
                response.Message = $"Unable to find achievement {id}";
            }
            else
            {
                response.Data = _mapper.Map<GetAchievementDto>(_repository.Get(id));
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> AddAchievement(AddAchievementDto newAchievement)
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();
            Achievement achievement = _mapper.Map<Achievement>(newAchievement);
            achievement.Id = Guid.NewGuid();
            achievement.LastUpdated = DateTime.UtcNow;
            await _repository.Add(achievement);
            response.Data = _mapper.Map<List<GetAchievementDto>>(await _repository.GetAll());
            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> UpdateAchievement(UpdateAchievementDto updatedAchievement)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            try
            {
                Achievement? achievement = await _repository.Get(update);

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
                var achievement = await _repository.Remove(id);
                response.Data = _mapper.Map<List<GetAchievementDto>>(_repository.GetAll());
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