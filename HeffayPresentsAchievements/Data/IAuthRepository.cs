using HeffayPresentsAchievements.Models;
using System;
using System.Security;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<Guid>> Register(User user, SecureString password);
        Task<ServiceResponse<string>> Login(string username, SecureString password);
        Task<bool> UserExists(string username);
    }
}
