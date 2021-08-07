using System.Collections.Generic;
using System.Threading.Tasks;
using HeffayPresentsAchievements.Models;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public interface IAchievementService
    {
         Task<ServiceResponse<List<Achievement>>> GetAllAchievements();
         Task<ServiceResponse<Achievement>> GetAchievementById(string id);
         Task<ServiceResponse<List<Achievement>>> AddAchievement(Achievement newAchievement);
    }
}