using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.Unit_Tests.Service_Tests
{
    [TestClass]
    public class GameServiceTests
    {
        private IMapper? mapper;
        private readonly Mock<IRepository<Achievement>> achievementRepo = new();
        private readonly Mock<IRepository<Game>> gameRepo = new();
        private readonly Mock<IHttpContextAccessor> context = new();
        private Guid badGuid = new("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

        [TestInitialize]
        public void Initialize()
        {
            var profile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(configuration);
        }

        [TestMethod]
        public async Task GetAllGames_ShouldReturnAllGames()
        {
            gameRepo.Setup(p => p.GetAll()).Returns(Seed()!);

            var service = new GameService(mapper!, achievementRepo.Object, gameRepo.Object, context.Object);
            var actualServiceResponse = await service.GetGames();

            var expectedServiceResponse = new ServiceResponse<List<GetGameDto>>
            {
                Data = new List<GetGameDto> { new GetGameDto(new Guid(), "game name") },
                Success = true,
                Message = null
            };

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.AreEqual(expectedServiceResponse.Data.Count, actualServiceResponse.Data.Count);
            Assert.AreEqual(expectedServiceResponse.Data[0].Name, actualServiceResponse.Data[0].Name);
            Assert.AreEqual(expectedServiceResponse.Data[0].GameId, actualServiceResponse.Data[0].GameId);
        }

        private async static Task<IEnumerable<Game?>> Seed()
        {
            var games = new List<Game?>
            {
                new Game
                {
                    Id = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"),
                    Name = "First achievement",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    LastUpdated = DateTime.UtcNow
                },
            };

            return games;
        }
    }
}
