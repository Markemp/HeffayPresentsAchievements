using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Dtos.Achievement;

namespace HeffayPresentsAchievements.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementService _service;

        public AchievementController(IAchievementService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetAchievementDto>>>> Get()
        {
            return Ok(await _service.GetAllAchievements());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetAchievementDto>>> GetSingle(string id)
        {
            return Ok(await _service.GetAchievementById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetAchievementDto>>>> AddAchievement(AddAchievementDto newAchievement)
        {
            return Ok(await _service.AddAchievement(newAchievement));
        }
    }
}