using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.IntegrationTests
{
    [TestClass]
    public class AchievementServiceIntegrationTests
    {
        private readonly DataContext _context;
        private readonly Mapper mapper;
        private readonly Repository<Achievement> _achievementRepo;
        private readonly Repository<Game> _gameRepo;
        private readonly HttpContextAccessor _httpContext = new();
        private readonly DbContextOptions<DataContext> options;

        public AchievementServiceIntegrationTests()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Achievements")
                .Options;
            _context = CreateContext(options);

            _achievementRepo = new(_context);
            _gameRepo = new(_context);

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });
            mapper = new(mapperConfig);
        }

        [TestMethod]
        public async Task AddAchievement_AddToTestDb()
        {
            // Verify db empty, add an achievement, verify it exists
            var newAchievement = new AddAchievementDto
            {
                Name = "Achievement 01",
                Points = 10,
                PercentageUnlocked = 0,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper, _achievementRepo, _gameRepo, _httpContext);

            var initialResult = await service.GetAllAchievements();
            Assert.IsTrue(initialResult.Success);
            Assert.IsNull(initialResult.Data);
            Assert.AreEqual("No achievements found.", initialResult.Message);

            var addResult = await service.AddAchievement(newAchievement);
            Assert.IsTrue(addResult.Success);
            Assert.AreEqual("Added 1 row (should be 1).", addResult.Message);
            Assert.AreEqual("Achievement 01", addResult.Data!.Name);

            var checkResult = await service.GetAllAchievements();
            Assert.IsTrue(checkResult.Success);
            Assert.IsNotNull(checkResult.Data);
            Assert.AreEqual(1, checkResult.Data!.Count);
            Assert.AreEqual("Achievement 01", addResult.Data!.Name);
        }

        [TestMethod]
        public async Task GetAchievementById()
        {
            // Verify db empty, add an achievement, verify it exists
            var newAchievement = new AddAchievementDto
            {
                Name = "Achievement 01",
                Points = 10,
                PercentageUnlocked = 0,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var newAchievement2 = new AddAchievementDto
            {
                Name = "Achievement 02",
                Points = 10,
                PercentageUnlocked = 0,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper, _achievementRepo, _gameRepo, _httpContext);

            var addResult = await service.AddAchievement(newAchievement);
            var addResult2 = await service.AddAchievement(newAchievement2);
            Assert.IsTrue(addResult.Success);
            Assert.IsTrue(addResult2.Success);
            Assert.AreEqual("Added 1 row (should be 1).", addResult.Message);
            Assert.AreEqual("Added 1 row (should be 1).", addResult2.Message);

            var id1 = addResult2.Data!.Id;
            var id2 = addResult2.Data!.Id;

            var checkResult = await service.GetAchievementById(id1);
            var checkResult2 = await service.GetAchievementById(id2);

            Assert.AreEqual(id1, checkResult.Data!.Id);
            Assert.AreEqual(id2, checkResult2.Data!.Id);
        }

        [TestMethod]
        public async Task UpdateAchievement()
        {
            // Verify db empty, add an achievement, verify it exists
            var achievement = new AddAchievementDto
            {
                Name = "Update Achievement Test",
                Points = 10,
                PercentageUnlocked = 0,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper, _achievementRepo, _gameRepo, _httpContext);

            var addResult = await service.AddAchievement(achievement);

            var updateResult = await service.UpdateAchievement(updateAchievement);
            Assert.IsTrue(updateResult.Success);
            Assert.AreEqual("Added 1 row (should be 1).", updateResult.Message);

            var checkResult = await service.GetAchievementById(id1);

        }

        private static DataContext CreateContext(DbContextOptions<DataContext> options)
            => new(options);
    }
}
