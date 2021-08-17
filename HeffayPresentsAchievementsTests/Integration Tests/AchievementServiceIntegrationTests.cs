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
        public async Task AddAchievement_AddToTestDb()
        {
            // Verify db empty, add an achievement, verify it exists
            var newAchievement = new AddAchievementDto
            {
                Name = "Achievement 01",
                Points = 10,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper!, _achievementRepo!, _gameRepo!, _httpContext!);

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
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var newAchievement2 = new AddAchievementDto
            {
                Name = "Achievement 02",
                Points = 10,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper!, _achievementRepo!, _gameRepo!, _httpContext!);

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
            var newAchievement = new AddAchievementDto
            {
                Name = "Update Achievement Test",
                Points = 10,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper!, _achievementRepo!, _gameRepo!, _httpContext!);

            var addResult = await service.AddAchievement(newAchievement);
            Assert.IsTrue(addResult.Success);

            var achievement = addResult.Data!;

            var updateAchievement = new UpdateAchievementDto
            {
                Id = achievement.Id,
                Name = achievement.Name,
                IsDeleted = achievement.IsDeleted,
                AchievementType = achievement.AchievementType,
                IsIncrementalAchievement = achievement.IsIncrementalAchievement,
                Points = achievement.Points
            };

            // No values changed
            var updateResult = await service.UpdateAchievement(updateAchievement);
            Assert.IsTrue(updateResult.Success);

            // Values changed
            updateAchievement.Name = "New Achievement name";
            updateAchievement.Points = 20;
            updateAchievement.IsIncrementalAchievement = false;
            updateAchievement.IsDeleted = true;

            updateResult = await service.UpdateAchievement(updateAchievement);
            Assert.AreEqual("New Achievement name", updateResult.Data!.Name);
            Assert.AreEqual(20, updateResult.Data!.Points);
            Assert.IsTrue(updateResult.Data!.IsDeleted);
            Assert.IsFalse(updateResult.Data!.IsIncrementalAchievement);

            // Values changed again
            updateAchievement.IsDeleted = false;
            updateAchievement.Name = "lol";
            var updateResult2 = await service.UpdateAchievement(updateAchievement);

            Assert.AreEqual("lol", updateResult2.Data!.Name);
            Assert.IsFalse(updateResult2.Data!.IsDeleted);
        }

        [TestMethod]
        public async Task DeleteAchievement()
        {
            // Verify db empty, add an achievement, verify it exists
            var newAchievement = new AddAchievementDto
            {
                Name = "Delete Achievement Test",
                Points = 10,
                IsIncrementalAchievement = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper!, _achievementRepo!, _gameRepo!, _httpContext!);

            var addResult = await service.AddAchievement(newAchievement);

            // Delete non-existant achievement
            var deleteResult = await service.DeleteAchievement(Guid.Empty);
            Assert.IsFalse(deleteResult.Success);
            Assert.AreEqual("Achievement not found.", deleteResult.Message);

            // Delete added achievement
            var deleteResult2 = await service.DeleteAchievement(addResult.Data!.Id);
            Assert.IsTrue(deleteResult2.Success);
            Assert.IsTrue(deleteResult2.Message!.StartsWith("Removed 1 rows."));
            var checkAchievement = await service.GetAchievementById(addResult.Data!.Id);
            Assert.IsFalse(checkAchievement.Success);
        }

        private static DataContext CreateContext(DbContextOptions<DataContext> options)
        => new(options);
    }
}
