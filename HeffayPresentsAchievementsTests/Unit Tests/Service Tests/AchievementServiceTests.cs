using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Dtos.Achievement;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
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
        public async Task GetAllAchievements_ShouldReturnAllAchievements()
        {
            repo.Setup(p => p.GetAll()).Returns(Seed()!);

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
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

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            var actualServiceResponse = await service.GetAllAchievements();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetAllAchievements_RepoUnavailable_FailedServiceResponse()
        {
            repo.Setup(p => p.GetAll()).Throws(new Exception("Test message"));

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
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
            
            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);

            var actualServiceResponse = await service.GetAchievementById(new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"));

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetAchievementById_AchievementNotFound_SuccessFalseAndNotFoundMessage()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult<Achievement?>(null));

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);

            var actualServiceResponse = await service.GetAchievementById(badGuid);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals($"Achievement {badGuid} not found."));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task GetAchievementById_RepoUnavailable_FailedServiceResponse()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Throws(new Exception("Test message"));

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            var actualServiceResponse = await service.GetAchievementById(badGuid);

            Assert.IsNull(actualServiceResponse.Data);
            Assert.AreEqual("Test message", actualServiceResponse.Message);
            Assert.IsFalse(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task AddAchievement_Success()
        {
            repo.Setup(p => p.Add(It.IsAny<Achievement>())).Returns(Task.FromResult(1));
            repo.Setup(p => p.GetAll()).Returns(Seed());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);

            var newAchievement = new AddAchievementDto
            {
                AchievementType = AchievementType.Visible,
                IsIncrementalAchievement = false,
                Name = "New Achievement",
                PercentageUnlocked = 0f,
                Points = 10
            };

            var actualServiceResponse = await service.AddAchievement(newAchievement);

            Assert.AreEqual(4, actualServiceResponse.Data!.Count);
            Assert.AreEqual("Added 1 record.", actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task AddAchievement_NullSubmitted_ThrowNullArgException()
        {
            repo.Setup(p => p.Add(It.IsAny<Achievement>())).Throws(new ArgumentNullException());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            AddAchievementDto? ach = null;

            var actualServiceResponse = await service.AddAchievement(ach!);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.AreEqual("Object reference not set to an instance of an object.", actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task AddAchievement_AchievementSubmitted_ThrowException()
        {
            repo.Setup(p => p.Add(It.IsAny<Achievement>())).Throws(new Exception());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            AddAchievementDto ach = new();

            var actualServiceResponse = await service.AddAchievement(ach);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.StartsWith("Exception of type "));
        }

        [TestMethod]
        public async Task UpdateAchievement_Success()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(Seed().Result.FirstOrDefault()));
            repo.Setup(p => p.Update(It.IsAny<Achievement>())).Returns(Task.FromResult(new Achievement()));

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            UpdateAchievementDto ach = new();

            var actualServiceResponse = await service.UpdateAchievement(ach);

            Assert.IsTrue(actualServiceResponse.Success);
            Assert.IsNull(actualServiceResponse.Message);
            Assert.IsNotNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task UpdateAchievement_AchievementNotFound()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult<Achievement?>(null));

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            UpdateAchievementDto ach = new();

            var actualServiceResponse = await service.UpdateAchievement(ach);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.StartsWith("Achievement "));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task UpdateAchievement_UpdateFailed()
        {
            repo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(Seed().Result.FirstOrDefault()));
            repo.Setup(p => p.Update(It.IsAny<Achievement>())).Throws(new Exception());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            UpdateAchievementDto ach = new();

            var actualServiceResponse = await service.UpdateAchievement(ach);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Exception of type 'System.Exception' was thrown."));
            Assert.IsNull(actualServiceResponse.Data);
        }


        [TestMethod]
        public async Task DeleteAchievement_Success()
        {
            repo.Setup(p => p.Remove(It.IsAny<Guid>())).Returns(Task.FromResult(1));
            repo.Setup(p => p.GetAll()).Returns(Seed());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            var idToDelete = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7");

            var actualServiceResponse = await service.DeleteAchievement(idToDelete);

            Assert.IsTrue(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Removed 1 rows."));
            Assert.AreEqual(4, actualServiceResponse.Data!.Count);
        }

        [TestMethod]
        public async Task DeleteAchievement_AchievementNotFound()
        {
            repo.Setup(p => p.Remove(It.IsAny<Guid>())).Returns(Task.FromResult(0));
            repo.Setup(p => p.GetAll()).Returns(Seed());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            var idToDelete = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

            var actualServiceResponse = await service.DeleteAchievement(idToDelete);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Achievement not found."));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task DeleteAchievement_ExceptionThrown()
        {
            repo.Setup(p => p.Remove(It.IsAny<Guid>())).Throws(new Exception());
            repo.Setup(p => p.GetAll()).Returns(Seed());

            var service = new AchievementService(mapper!, repo.Object, gameRepo.Object, context.Object);
            var idToDelete = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

            var actualServiceResponse = await service.DeleteAchievement(idToDelete);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Exception of type 'System.Exception' was thrown."));
            Assert.IsNull(actualServiceResponse.Data);
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
