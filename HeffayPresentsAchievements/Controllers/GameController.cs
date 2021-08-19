using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.GameService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Controllers
{
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
            return Ok(await _service.GetAllGames());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetGameDto>>> GetGame(Guid id)
        {
            return Ok(await _service.GetGameById(id));
        }
    }
}
