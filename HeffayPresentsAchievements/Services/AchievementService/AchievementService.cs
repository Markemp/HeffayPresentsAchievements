using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using Microsoft.EntityFrameworkCore;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public class AchievementService : IAchievementService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public AchievementService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var dbAchievements = await _context.Achievements.ToListAsync();
            var response = new ServiceResponse<List<GetAchievementDto>>
            {
                Data = dbAchievements.Where(a => a.IsDeleted == false).Select(a => _mapper.Map<GetAchievementDto>(a)).ToList()
            };
            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> GetAchievementById(Guid id)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (achievement == null)
            {
                response.Success = false;
                response.Message = $"Unable to find achievement {id}";
            }
            else
            {
                response.Data = _mapper.Map<GetAchievementDto>(_context.Achievements.FirstOrDefault(a => a.Id.Equals(id)));
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> AddAchievement(AddAchievementDto newAchievement)
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();
            Achievement achievement = _mapper.Map<Achievement>(newAchievement);
            achievement.Id = Guid.NewGuid();
            achievement.LastUpdated = DateTime.UtcNow;
            _context.Achievements.Add(achievement);
            await _context.SaveChangesAsync();
            response.Data = await _context.Achievements.Select(a => _mapper.Map<GetAchievementDto>(a)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> UpdateAchievement(UpdateAchievementDto updatedAchievement)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            try
            {
                Achievement? achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Id.Equals(updatedAchievement.Id));

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
                var achievements = _context.Achievements.Where(a => a.Id.Equals(id));
                _context.Achievements.RemoveRange(achievements);
                await _context.SaveChangesAsync();
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