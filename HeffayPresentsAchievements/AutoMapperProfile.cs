using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;

namespace HeffayPresentsAchievements
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Achievement, GetAchievementDto>().ReverseMap();
            CreateMap<Achievement, AddAchievementDto>().ReverseMap();
            CreateMap<Achievement, UpdateAchievementDto>().ReverseMap();
        }
    }
}
