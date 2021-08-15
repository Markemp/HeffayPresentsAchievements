using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace HeffayPresentsAchievementsTests.IntegrationTests
{
    [TestClass]
    public class AchievementServiceIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly DataContext _context;
        private readonly IMapper? mapper;

        public AchievementServiceIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Achievements")
                .Options;
            _context = CreateContext(options);
        }

        [Fact]
        public async Task AddAchievement_AddedToTestDb()
        {
            var newAchievement = new Achievement
            {
                Id = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Name = "Achievement 01",
                Points = 10,
                PercentageUnlocked = 0,
                IsIncrementalAchievement = false,
                IsDeleted = false,
                AchievementType = AchievementType.Visible
            };

            var service = new AchievementService(mapper!, null, null, null);
            await service.AddAchievement(new AddAchievementDto());

        }
        private static DataContext CreateContext(DbContextOptions<DataContext> options)
            => new(options);
    }
}
