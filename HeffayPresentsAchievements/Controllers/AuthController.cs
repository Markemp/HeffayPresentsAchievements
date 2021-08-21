using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<Guid>>> Register(UserRegisterDto request)
        {
            var response = await _authRepo.Register(new User { Username = request.Username}, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
