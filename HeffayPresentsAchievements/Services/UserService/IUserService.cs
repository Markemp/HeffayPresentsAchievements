using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Models.Dtos.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<List<GetUserDto>>> GetAllUsers();
        Task<ServiceResponse<List<GetGameDto>>> GetAllGames();
        Task<ServiceResponse<GetUserDto>> UpdateUser(UpdateUserDto updatedUser);
    }
}
