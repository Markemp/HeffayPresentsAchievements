using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;

namespace HeffayPresentsAchievements
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Achievement, GetAchievementDto>();
            CreateMap<AddAchievementDto, Achievement>();
        }
    }
}
