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

        private DbContextOptions<DataContext> options;

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

            var service = new AchievementService(mapper, _achievementRepo, _gameRepo, _httpContext);
            
            await service.AddAchievement(new AddAchievementDto());
        
        }
        private static DataContext CreateContext(DbContextOptions<DataContext> options)
            => new(options);
    }
}
