using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeffayPresentsAchievements.Models;

namespace HeffayPresentsAchievements.Services.AchievementService
{
    public class AchievementService : IAchievementService
    {
        private static List<Achievement> achievements = new List<Achievement> 
        {
            new Achievement { Id = "0" },
            new Achievement { Name = "Second achievement", Id = "1"}
        };
        
        public async Task<ServiceResponse<List<Achievement>>> AddAchievement(Achievement newAchievement)
        {
            var response = new ServiceResponse<List<Achievement>>();
            achievements.Add(newAchievement);
            response.Data = achievements;
            return response;
        }

        public async Task<ServiceResponse<Achievement>> GetAchievementById(string id)
        {
            var response = new ServiceResponse<Achievement>();
            response.Data = achievements.FirstOrDefault(a => a.Id.Equals(id));
            return response;
        }

        public async Task<ServiceResponse<List<Achievement>>> GetAllAchievements()
        {
            var response = new ServiceResponse<List<Achievement>>();
            response.Data = achievements;
            return response;
        }
    }
}