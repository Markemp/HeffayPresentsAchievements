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
        private static List<Achievement> achievements = new List<Achievement> 
        {
            new Achievement { Id = "0" },
            new Achievement { Name = "Second achievement", Id = "1"}
        };
        private readonly IMapper _mapper;

        public AchievementService(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        public async Task<ServiceResponse<List<GetAchievementDto>>> AddAchievement(AddAchievementDto newAchievement)
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();
            achievements.Add(_mapper.Map<Achievement>(newAchievement));
            response.Data = achievements.Select(a => _mapper.Map<GetAchievementDto>(a)).ToList();
            return response;
        }

        public async Task<ServiceResponse<List<GetAchievementDto>>> GetAllAchievements()
        {
            var response = new ServiceResponse<List<GetAchievementDto>>();
            response.Data = achievements.Select(a => _mapper.Map<GetAchievementDto>(a)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetAchievementDto>> GetAchievementById(string id)
        {
            var response = new ServiceResponse<GetAchievementDto>();
            response.Data = _mapper.Map<GetAchievementDto>(achievements.FirstOrDefault(a => a.Id.Equals(id)));
            return response;
        }
    }
}