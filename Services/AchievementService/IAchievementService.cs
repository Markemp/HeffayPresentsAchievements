using System.Collections.Generic;
using System.Threading.Tasks;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public interface IAchievementService
    {
         Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements();
         Task<ServiceResponse<GetAchievementDto>> GetAchievementById(string id);
         Task<ServiceResponse<List<GetAchievementDto>>> AddAchievement(AddAchievementDto newAchievement);
    }
}