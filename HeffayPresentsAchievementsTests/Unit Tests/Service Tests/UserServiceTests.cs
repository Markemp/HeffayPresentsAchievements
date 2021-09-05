using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HeffayPresentsAchievementsTests.Unit_Tests.Service_Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private IMapper? mapper;
        private readonly Mock<IRepository<Achievement>> achievementRepo = new();
        private readonly Mock<IRepository<Game>> gameRepo = new();

        [TestInitialize]
        public void Initialize()
        {
            var profile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(configuration);
        }
    }
}
