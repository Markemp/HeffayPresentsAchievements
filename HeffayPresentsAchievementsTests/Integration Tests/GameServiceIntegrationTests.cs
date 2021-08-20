using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.Integration_Tests
{
    [TestClass]
    public class GameServiceIntegrationTests
    {
        private DataContext? _context;
        private Mapper? mapper;
        private Repository<Achievement>? _achievementRepo;
        private Repository<Game>? _gameRepo;
        private HttpContextAccessor? _httpContext = new();
        private DbContextOptions<DataContext>? options;

        [TestInitialize]
        public void Initialize()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Achievements")
                .Options;
            _context = CreateContext(options);

            _achievementRepo = new(_context);
            _gameRepo = new(_context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            mapper = new(mapperConfig);
        }

        [TestMethod]
        public async Task AddGame_AddToTestDb()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame = new AddGameDto("Game 01", new Guid());

            var service = new GameService(mapper!, _achievementRepo!, _gameRepo!, _httpContext!);

            var initialResult = await service.GetAllGames();
            Assert.IsTrue(initialResult.Success);
            Assert.IsNull(initialResult.Data);
            Assert.AreEqual("No games found.", initialResult.Message);

            var addResult = await service.AddGame(newGame);
            Assert.IsTrue(addResult.Success);
            Assert.AreEqual("Added 1 row (should be 1).", addResult.Message);
            Assert.AreEqual("Game 01", addResult.Data!.Name);

            var checkResult = await service.GetAllGames();
            Assert.IsTrue(checkResult.Success);
            Assert.IsNotNull(checkResult.Data);
            Assert.AreEqual(1, checkResult.Data!.Count);
            Assert.AreEqual("Game 01", addResult.Data!.Name);
        }

        [TestMethod]
        public async Task GetGameById()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame1 = new AddGameDto("Game 01", new Guid());
            var newGame2 = new AddGameDto("Game 02", new Guid());

            var service = new GameService(mapper!, _achievementRepo!, _gameRepo!, _httpContext!);

            var addResult = await service.AddGame(newGame1);
            var addResult2 = await service.AddGame(newGame2);
            Assert.IsTrue(addResult.Success);
            Assert.IsTrue(addResult2.Success);
            Assert.AreEqual("Added 1 row (should be 1).", addResult.Message);
            Assert.AreEqual("Added 1 row (should be 1).", addResult2.Message);

            var id1 = addResult2.Data!.Id;
            var id2 = addResult2.Data!.Id;

            var checkResult = await service.GetGameById(id1);
            var checkResult2 = await service.GetGameById(id2);

            Assert.AreEqual(id1, checkResult.Data!.Id);
            Assert.AreEqual(id2, checkResult2.Data!.Id);
        }

        private static DataContext CreateContext(DbContextOptions<DataContext> options) => new(options);
    }
}
