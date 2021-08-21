using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IRepository<User> _userRepo;

        public AuthRepository(DataContext context, IRepository<User> userRepository)
        {
            _context = context;
            _userRepo = userRepository;
        }

        public async Task<ServiceResponse<string>> Login(string username, SecureString password)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<Guid>> Register(User user, SecureString password)
        {
            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();
            //ServiceResponse
            throw new NotImplementedException();
        }

        public async Task<bool> UserExists(string username)
        {
            throw new NotImplementedException();
        }
    }
}
