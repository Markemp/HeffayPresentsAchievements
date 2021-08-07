using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Dtos.Achievement;
using System;

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
        public async Task<ActionResult<ServiceResponse<GetAchievementDto>>> GetSingle(Guid id)
        {
            return Ok(await _service.GetAchievementById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetAchievementDto>>>> AddAchievement(AddAchievementDto newAchievement)
        {
            return Ok(await _service.AddAchievement(newAchievement));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetAchievementDto>>> UpdateAchievement(UpdateAchievementDto updatedAchievement)
        {
            var response = await _service.UpdateAchievement(updatedAchievement);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<List<GetAchievementDto>>>> DeleteAchievement(Guid id)
        {
            var response = await _service.DeleteAchievement(id);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}