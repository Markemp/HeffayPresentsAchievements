using AutoMapper;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;

namespace HeffayPresentsAchievements
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Achievement, GetAchievementDto>().ReverseMap();
            CreateMap<GetAchievementDto, Achievement>().ReverseMap();
            CreateMap<Achievement, AddAchievementDto>().ReverseMap();

            CreateMap<Game, GetGameDto>().ReverseMap();
            CreateMap<Game, AddGameDto>().ReverseMap();

        }
    }
}
