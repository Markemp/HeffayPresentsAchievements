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
        private readonly IMapper _mapper;
        private readonly IRepository<Achievement> _repository;
        private readonly IHttpContextAccessor _httpContext;

        public AchievementService(IMapper mapper, IRepository<Achievement> repo, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _repository = repo;
            _httpContext = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();

            try
            {
                var dbAchievements = await _repository.GetAll();

                if (dbAchievements.Any())
                    response.Data = dbAchievements.Where(a => a != null && a.IsDeleted == false).Select(a => _mapper.Map<GetAchievementDto>(a)).ToList();
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
                var achievement = await _repository.Get(id);
                if (achievement == null)
                {
                    response.Message = $"Achievement {id} not found.";
                    response.Success = false;
                }
                else
                {
                    response.Data = _mapper.Map<GetAchievementDto>(await _repository.Get(id));
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

        public async Task<ServiceResponse<List<GetAchievementDto>>> AddAchievement(AddAchievementDto newAchievement)
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();

            try
            {
                Achievement achievement = _mapper.Map<Achievement>(newAchievement);
                achievement.Id = Guid.NewGuid();
                achievement.LastUpdated = DateTime.UtcNow;
                achievement.Game = new();
                int rowsChanged = await _repository.Add(achievement);
                response.Data = _mapper.Map<List<GetAchievementDto>>(await _repository.GetAll());
                response.Message = $"Added {rowsChanged} record.";
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
                Achievement? achievement = await _repository.Get(updatedAchievement.Id);

                if (achievement != null)
                {
                    achievement.Name = updatedAchievement.Name ?? achievement.Name;
                    achievement.IsIncrementalAchievement = updatedAchievement.IsIncrementalAchievement ?? achievement.IsIncrementalAchievement;
                    achievement.PercentageUnlocked = updatedAchievement.PercentageUnlocked ?? achievement.PercentageUnlocked;
                    achievement.Points = updatedAchievement.Points ?? achievement.Points;
                    achievement.AchievementType = updatedAchievement.AchievementType ?? achievement.AchievementType;
                    achievement.LastUpdated = DateTime.UtcNow;
                    await _repository.Update(achievement);

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