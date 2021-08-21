using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

            var service = new GameService(mapper!, _achievementRepo!, _gameRepo!);

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

            var service = new GameService(mapper!, _achievementRepo!, _gameRepo!);

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

        [TestMethod]
        public async Task UpdateGame()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame = new AddGameDto("Update Game Test",new Guid());

            var service = new GameService(mapper!, _achievementRepo!, _gameRepo!);

            var addResult = await service.AddGame(newGame);
            Assert.IsTrue(addResult.Success);

            var game = addResult.Data!;

            var updateGame = new UpdateGameDto(game.Id, game.Name);

            // No values changed
            var updateResult = await service.UpdateGame(updateGame);
            Assert.IsTrue(updateResult.Success);
            Assert.AreEqual("Update Game Test", updateResult.Data!.Name);

            // Values changed
            var updateGame2 = new UpdateGameDto(game.Id, "Updated Name");

            updateResult = await service.UpdateGame(updateGame2);
            Assert.AreEqual("Updated Name", updateResult.Data!.Name);

            // Values changed again
            var updateGame3 = new UpdateGameDto(game.Id, "New Updated Name");
            var updateResult2 = await service.UpdateGame(updateGame3);

            Assert.AreEqual("New Updated Name", updateResult2.Data!.Name);
            Assert.AreEqual(updateGame.Id, updateResult2.Data.Id);
        }

        [TestMethod]
        public async Task DeleteGame()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame = new AddGameDto("Delete Game Test", new Guid());

            var service = new GameService(mapper!, _achievementRepo!, _gameRepo!);

            var addResult = await service.AddGame(newGame);

            // Delete non-existant achievement
            var deleteResult = await service.DeleteGame(Guid.Empty);
            Assert.IsFalse(deleteResult.Success);
            Assert.AreEqual("Game not found.", deleteResult.Message);

            // Delete added achievement
            var deleteResult2 = await service.DeleteGame(addResult.Data!.Id);
            Assert.IsTrue(deleteResult2.Success);
            Assert.IsTrue(deleteResult2.Message!.StartsWith("Removed 1 rows."));
            var checkAchievement = await service.GetGameById(addResult.Data!.Id);
            Assert.IsFalse(checkAchievement.Success);
        }

        private static DataContext CreateContext(DbContextOptions<DataContext> options) => new(options);
    }
}
