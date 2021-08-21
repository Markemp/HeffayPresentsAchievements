using HeffayPresentsAchievements.Models;
using System;
using System.Security;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<Guid>> Register(User user, string password);
        Task<ServiceResponse<string>> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}
