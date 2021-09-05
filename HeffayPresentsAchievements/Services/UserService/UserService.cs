using AutoMapper;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Models.Dtos.User;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepo;
        private readonly IGameService _gameService;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IMapper mapper,
            IRepository<User> userRepository,
            IGameService gameService,
            IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _userRepo = userRepository;
            _gameService = gameService;
            _httpContext = httpContext;
        }
        
        private Guid GetUserId()
        {
            if (_httpContext.HttpContext != null)
            {
                var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.Parse(userId);
            }

            else
                throw new ApplicationException("Unable to find HttpContext for request.");
        }

        public async Task<ServiceResponse<List<GetGameDto>>> GetAllGames()
        {
            var response = new ServiceResponse<List<GetGameDto>>();

            try
            {
                var allGames = await _gameService.GetAllGames();
                if (allGames.Success == true)
                {
                    if (allGames.Data != null)
                    {
                        response.Data = allGames.Data
                        .Where(g => g != null)
                        .Select(g => _mapper.Map<GetGameDto>(g))
                        .ToList();
                    }
                    else
                    {
                        response.Message = "No games found.";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = allGames.Message;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
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

        public async Task<ServiceResponse<GetUserDto>> UpdateUser(UpdateUserDto updatedUser)
        {
            var response = new ServiceResponse<GetUserDto>();

            try
            {
                var user = await _userRepo.Get(updatedUser.Id);

                if (user != null)
                {
                    user.Username = updatedUser.Name;
                    user.Email = updatedUser.Email;

                    response.Data = _mapper.Map<GetUserDto>(user);
                }
                else
                {
                    response.Success = false;
                    response.Message = $"User {updatedUser.Id} not found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
