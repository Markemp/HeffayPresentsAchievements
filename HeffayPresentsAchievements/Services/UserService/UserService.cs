using AutoMapper;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Models.Dtos.User;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepo;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IMapper mapper,
            IRepository<User> userRepository,
            IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _userRepo = userRepository;
            _httpContext = httpContext;
        }

        public Task<ServiceResponse<GetGameDto>> AddGameToUser(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<GetUserDto>>> GetAllUsers()
        {
            var response = new ServiceResponse<List<GetUserDto>>();

            try
            {
                var allUsers = await _userRepo.GetAll();
                if (allUsers.Any())
                {
                    response.Data = allUsers
                        .Where(g => g != null)
                        .Select(g => _mapper.Map<GetUserDto>(g))
                        .ToList();
                }
                else
                {
                    response.Success = true;
                    response.Message = "No users found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public Task<ServiceResponse<List<GetGameDto>>> RemoveGameFromUser(Guid gameId)
        {
            throw new NotImplementedException();
        }
    }
}
