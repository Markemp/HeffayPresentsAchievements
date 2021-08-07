using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Controllers;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.ControllersTests
{
    [TestClass]
    public class AchievementServiceTests
    {
        private IMapper mapper;

        [TestInitialize]
        public void Initialize()
        {
            var profile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(configuration);
        }

        [TestMethod]
        public async Task GetAllAchievements_ShouldReturnAllAchievements()
        {
            var service = new AchievementService(mapper);

            var actualServiceResponse = await service.GetAllAchievements();

            var expectedServiceResponse = new ServiceResponse<List<GetAchievementDto>>
            {
                Data = GetExpectedAchievements(),
                Success = true,
                Message = null
            };

            Assert.AreEqual(expectedServiceResponse.Data.Count, actualServiceResponse.Data.Count);
            Assert.AreEqual(expectedServiceResponse.Data[0].Name, actualServiceResponse.Data[0].Name);
            Assert.AreEqual(expectedServiceResponse.Data[0].Id, actualServiceResponse.Data[0].Id);
        }

        [TestMethod]
        public async Task GetAchievementById_ReturnsCorrectAchievement()
        {
            var service = new AchievementService(mapper);
            var id = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7");

            var expectedServiceResponse = new ServiceResponse<GetAchievementDto>
            {
                Data = new GetAchievementDto
                {
                    Name = "First achievement",
                    Id = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7")
                },
                Success = true,
                Message = null
            };

            var actualServiceResponse = await service.GetAchievementById(id);

            Assert.AreEqual(expectedServiceResponse.Data.Name, actualServiceResponse.Data.Name);
            Assert.AreEqual(expectedServiceResponse.Data.Id, actualServiceResponse.Data.Id);
        }

        [TestMethod]
        public async Task GetAchievementById_AchievementNotFound()
        {
            var service = new AchievementService(mapper);
            var id = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

            var expectedServiceResponse = new ServiceResponse<GetAchievementDto>
            {
                Data = null,
                Success = false,
                Message = $"Unable to find achievement {id}"
            };

            var actualServiceResponse = await service.GetAchievementById(id);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message.Equals($"Unable to find achievement {id}"));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task AddAchievement_Success()
        {
            var service = new AchievementService(mapper);
            var newAchievement = new AddAchievementDto
            {
                AchievementType = AchievementType.Visible,
                IsIncrementalAchievement = false,
                Name = "New Achievement",
                PercentageUnlocked = 0f,
                Points = 10
            };

            var actualServiceResponse = await service.AddAchievement(newAchievement);

            Assert.AreEqual(3, actualServiceResponse.Data.Count);
            Assert.IsNull(actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task DeleteAchievement_Success()
        {
            var service = new AchievementService(mapper);
            var idToDelete = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7");

            var actualServiceResponse = await service.DeleteAchievement(idToDelete);

            Assert.IsTrue(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message.Equals("Removed 1 achievements."));
            Assert.AreEqual(1, actualServiceResponse.Data.Count);
        }

        [TestMethod]
        public async Task DeleteAchievement_AchievementNotFound()
        {
            var service = new AchievementService(mapper);
            var idToDelete = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

            var actualServiceResponse = await service.DeleteAchievement(idToDelete);

            Assert.IsTrue(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message.Equals("Removed 0 achievements."));
            Assert.AreEqual(2, actualServiceResponse.Data.Count);
        }

        private static List<GetAchievementDto> GetExpectedAchievements()
        {
            var achievements = new List<GetAchievementDto>
            {
                new GetAchievementDto
                {
                    Id = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"),
                    Name = "First achievement",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                },
                new GetAchievementDto
                {
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e340"),
                    Name = "Second achievement",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                },
            };

            return achievements;
        }

        private static List<Achievement> GetTestAchievements()
        {
            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Id = new Guid("16424f0b-1d06-4b54-93fa-d761c43ede37"),
                    Name = "Achievement 1",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                },
                new Achievement
                {
                    Id = new Guid("065e2a2d-d5fc-4c43-8fee-511e58ee0fd5"),
                    Name = "Achievement 2",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 20
                },
                new Achievement
                {
                    Id = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"),
                    Name = "Achievement 3 - deleted",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 30
                },
                new Achievement
                {
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e340"),
                    Name = "Achievement 4",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 40
                },

            };

            return achievements;
        }
    }
}
