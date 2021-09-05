using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.IntegrationTests
{
    [TestClass]
    public class GameServiceIntegrationTests
    {
        private IGameService? gameService; 
        
        private DataContext? _context;
        private HttpContextAccessor? _httpContext;
        private Mapper? mapper;

        private IAchievementService? _achievementService;
        private Repository<Game>? _gameRepo;
        private readonly Repository<Achievement>? _achievementRepo;
        private DbContextOptions<DataContext>? options;

        [TestInitialize]
        public void Initialize()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Achievements")
                .Options;
            _context = CreateContext(options);

            _gameRepo = new(_context);
            
            gameService = new GameService(mapper!, _achievementService!, _gameRepo!, _httpContext!);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            mapper = new(mapperConfig);

            _achievementService = new AchievementService(mapper, _achievementRepo!);

            _httpContext = new();
        }

        [TestMethod]
        public async Task AddGame_AddToTestDb()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame = new AddGameDto("Game 01", new Guid());

            var initialResult = await gameService!.GetAllGames();
            Assert.IsTrue(initialResult.Success);
            Assert.IsNull(initialResult.Data);
            Assert.AreEqual("No games found.", initialResult.Message);

            var addResult = await gameService.AddGame(newGame);
            Assert.IsTrue(addResult.Success);
            Assert.AreEqual("Added 1 row (should be 1).", addResult.Message);
            Assert.AreEqual("Game 01", addResult.Data!.Name);

            var checkResult = await gameService.GetAllGames();
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


            var addResult = await gameService!.AddGame(newGame1);
            var addResult2 = await gameService.AddGame(newGame2);
            Assert.IsTrue(addResult.Success);
            Assert.IsTrue(addResult2.Success);
            Assert.AreEqual("Added 1 row (should be 1).", addResult.Message);
            Assert.AreEqual("Added 1 row (should be 1).", addResult2.Message);

            var id1 = addResult2.Data!.Id;
            var id2 = addResult2.Data!.Id;

            var checkResult = await gameService.GetGameById(id1);
            var checkResult2 = await gameService.GetGameById(id2);

            Assert.AreEqual(id1, checkResult.Data!.Id);
            Assert.AreEqual(id2, checkResult2.Data!.Id);
        }

        [TestMethod]
        public async Task UpdateGame()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame = new AddGameDto("Update Game Test",new Guid());

            var addResult = await gameService!.AddGame(newGame);
            Assert.IsTrue(addResult.Success);

            var game = addResult.Data!;

            var updateGame = new UpdateGameDto(game.Id, game.Name);

            // No values changed
            var updateResult = await gameService.UpdateGame(updateGame);
            Assert.IsTrue(updateResult.Success);
            Assert.AreEqual("Update Game Test", updateResult.Data!.Name);

            // Values changed
            var updateGame2 = new UpdateGameDto(game.Id, "Updated Name");

            updateResult = await gameService.UpdateGame(updateGame2);
            Assert.AreEqual("Updated Name", updateResult.Data!.Name);

            // Values changed again
            var updateGame3 = new UpdateGameDto(game.Id, "New Updated Name");
            var updateResult2 = await gameService.UpdateGame(updateGame3);

            Assert.AreEqual("New Updated Name", updateResult2.Data!.Name);
            Assert.AreEqual(updateGame.Id, updateResult2.Data.Id);
        }

        [TestMethod]
        public async Task DeleteGame()
        {
            // Verify db empty, add an achievement, verify it exists
            var newGame = new AddGameDto("Delete Game Test", new Guid());

            var addResult = await gameService!.AddGame(newGame);

            // Delete non-existant achievement
            var deleteResult = await gameService.DeleteGame(Guid.Empty);
            Assert.IsFalse(deleteResult.Success);
            Assert.AreEqual("Game not found.", deleteResult.Message);

            // Delete added achievement
            var deleteResult2 = await gameService.DeleteGame(addResult.Data!.Id);
            Assert.IsTrue(deleteResult2.Success);
            Assert.IsTrue(deleteResult2.Message!.StartsWith("Removed 1 rows."));
            var checkAchievement = await gameService.GetGameById(addResult.Data!.Id);
            Assert.IsFalse(checkAchievement.Success);
        }

        private static DataContext CreateContext(DbContextOptions<DataContext> options) => new(options);
    }
}
