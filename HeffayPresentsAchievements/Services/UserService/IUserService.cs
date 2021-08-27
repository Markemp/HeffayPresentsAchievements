using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Models.Dtos.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<List<GetUserDto>>> GetAllUsers();
        Task<ServiceResponse<GetUserDto>> Login(UserLoginDto user);
        Task<ServiceResponse<GetGameDto>> AddGameToUser(Guid gameId);
        Task<ServiceResponse<List<GetGameDto>>> RemoveGameFromUser(Guid gameId);
    }
}
