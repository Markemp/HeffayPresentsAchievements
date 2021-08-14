using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.UnitTests.ServicesTests
{
    [TestClass]
    public class AchievementServiceTests
    {
        private IMapper? mapper;
        private readonly Mock<IRepository<Achievement>> repo = new();
        private Guid badGuid = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

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
            repo.Setup(p => p.GetAll()).Returns(Seed()!);

            var service = new AchievementService(mapper!, repo.Object);
            var actualServiceResponse = await service.GetAllAchievements();

            var expectedServiceResponse = new ServiceResponse<List<GetAchievementDto>>
            {
                Data = GetExpectedAchievements(),
                Success = true,
                Message = null
            };

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.AreEqual(expectedServiceResponse.Data.Count, actualServiceResponse.Data.Count);
            Assert.AreEqual(expectedServiceResponse.Data[0].Name, actualServiceResponse.Data[0].Name);
            Assert.AreEqual(expectedServiceResponse.Data[0].Id, actualServiceResponse.Data[0].Id);
        }

        [TestMethod]
        public async Task GetAllAchievements_NoAchievementsReturned()
        {
            repo.Setup(p => p.GetAll()).Returns(EmptyAchievementsList()!);

            var service = new AchievementService(mapper!, repo.Object);
            var actualServiceResponse = await service.GetAllAchievements();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetAllAchievements_RepoUnavailable_FailedServiceResponse()
        {
            repo.Setup(p => p.GetAll()).Throws(new Exception("Test message"));

            var service = new AchievementService(mapper!, repo.Object);
            var actualServiceResponse = await service.GetAllAchievements();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.AreEqual("Test message", actualServiceResponse.Message);
            Assert.IsFalse(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetAchievementById_Successful()
        {
            GetAchievementDto expectedAchievementDto = mapper!.Map<GetAchievementDto>(Seed().Result.ToList()[0]);

            repo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(Seed().Result.FirstOrDefault()));
            
            var service = new AchievementService(mapper!, repo.Object);

            var actualServiceResponse = await service.GetAchievementById(new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"));

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetAchievementById_AchievementNotFound_SuccessFalseAndNotFoundMessage()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult<Achievement?>(null));

            var service = new AchievementService(mapper!, repo.Object);

            var actualServiceResponse = await service.GetAchievementById(badGuid);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals($"Achievement {badGuid} not found."));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task GetAchievementById_RepoUnavailable_FailedServiceResponse()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Throws(new Exception("Test message"));

            var service = new AchievementService(mapper!, repo.Object);
            var actualServiceResponse = await service.GetAchievementById(badGuid);

            Assert.IsNull(actualServiceResponse.Data);
            Assert.AreEqual("Test message", actualServiceResponse.Message);
            Assert.IsFalse(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task AddAchievement_Success()
        {

            var service = new AchievementService(mapper!, repo.Object);
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

        //[TestMethod]
        //public async Task DeleteAchievement_Success()
        //{
        //    var service = new AchievementService(mapper, mockContext.Object);
        //    var idToDelete = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7");

        //    var actualServiceResponse = await service.DeleteAchievement(idToDelete);

        //    Assert.IsTrue(actualServiceResponse.Success);
        //    Assert.IsTrue(actualServiceResponse.Message.Equals("Removed 1 achievements."));
        //    Assert.AreEqual(1, actualServiceResponse.Data.Count);
        //}

        //[TestMethod]
        //public async Task DeleteAchievement_AchievementNotFound()
        //{
        //    var service = new AchievementService(mapper, mockContext.Object);
        //    var idToDelete = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

        //    var actualServiceResponse = await service.DeleteAchievement(idToDelete);

        //    Assert.IsTrue(actualServiceResponse.Success);
        //    Assert.IsTrue(actualServiceResponse.Message.Equals("Removed 0 achievements."));
        //    Assert.AreEqual(2, actualServiceResponse.Data.Count);
        //}

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
                new GetAchievementDto
                {
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e341"),
                    Name = "Third achievement",
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
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e343"),
                    Name = "Fifth achievement (hidden)",
                    AchievementType = AchievementType.Hidden,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                }
            };

            return achievements;
        }

        private async static Task<IEnumerable<Achievement?>> Seed()
        {
            var achievements = new List<Achievement?>
            {
                new Achievement
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
                new Achievement
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
                new Achievement
                {
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e341"),
                    Name = "Third achievement",
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
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e342"),
                    Name = "Fourth achievement",
                    AchievementType = AchievementType.Visible,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                },
                new Achievement
                {
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e343"),
                    Name = "Fifth achievement (hidden)",
                    AchievementType = AchievementType.Hidden,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                },
                new Achievement
                {
                    Id = new Guid("7d06a5de-bd52-439b-bfc5-7fbd7391e344"),
                    Name = "Sixth achievement (hidden, deleted)",
                    AchievementType = AchievementType.Hidden,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                    IsIncrementalAchievement = false,
                    LastUpdated = DateTime.UtcNow,
                    PercentageUnlocked = 0f,
                    Points = 10
                }
            };

            return achievements.AsEnumerable();
        }

        private async static Task<IEnumerable<Achievement?>> EmptyAchievementsList()
        {
            var achievements = new List<Achievement?>();
            return achievements.AsEnumerable();
        }
    }

}
