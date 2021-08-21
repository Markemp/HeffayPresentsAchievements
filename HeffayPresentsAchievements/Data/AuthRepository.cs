using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IRepository<User> _userRepo;
        private readonly IConfiguration _config;

        public AuthRepository(DataContext context, IRepository<User> userRepository, IConfiguration configuration)
        {
            _context = context;
            _userRepo = userRepository;
            _config = configuration;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));

            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid username or password";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash!, user.PasswordSalt!))
            {
                response.Success = false;
                response.Message = "Invalid username or password";
            }
            else
            {
                response.Data = CreateToken(user);
            }

            return response;
        }

        public async Task<ServiceResponse<Guid>> Register(User user, string password)
        {
            ServiceResponse<Guid> response = new();

            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.DateCreated = DateTime.UtcNow;
            user.LastUpdated = DateTime.UtcNow;

            await _userRepo.Add(user);
            response.Data = user.Id;
         
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users!.AnyAsync(x => x.Username.Equals(username)))
                return true;
            else
                return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] !=passwordHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // TODO:  Get this from usersecrets
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
