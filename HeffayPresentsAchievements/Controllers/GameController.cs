using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.GameService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _service;

        public GameController(IGameService gameService)
        {
            _service = gameService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetGameDto>>>> GetGames()
        {
            Guid id = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _service.GetAllGames());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetGameDto>>> GetGame(Guid id)
        {
            var response = await _service.GetGameById(id);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetGameDto>>> AddGame(AddGameDto game)
        {
            return Ok(await _service.AddGame(game));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetGameDto>>> UpdateAchievement(UpdateGameDto updatedGame)
        {
            var response = await _service.UpdateGame(updatedGame);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<List<Game>>>> DeleteGame(Guid id)
        {
            var response = await _service.DeleteGame(id);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
