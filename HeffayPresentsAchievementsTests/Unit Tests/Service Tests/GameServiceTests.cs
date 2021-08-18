﻿using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.Unit_Tests.Service_Tests
{
    [TestClass]
    public class GameServiceTests
    {
        private IMapper? mapper;
        private readonly Mock<IRepository<Achievement>> achievementRepo = new();
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
        public async Task GetAllGames_ShouldReturnAllGames()
        {
            gameRepo.Setup(p => p.GetAll()).Returns(Seed()!);

            var service = new GameService(mapper!, achievementRepo.Object, gameRepo.Object, context.Object);
            var actualServiceResponse = await service.GetAllGames();

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.AreEqual(2, actualServiceResponse.Data.Count);
            Assert.AreEqual("Game 1", actualServiceResponse.Data[0].Name);
            Assert.AreEqual(new Guid("6a3dbb1c-0001-41c4-9e84-410f17b644e7"), actualServiceResponse.Data[0].Id);
        }

        [TestMethod]
        public async Task GetAllGames_NoGamesReturned()
        {
            gameRepo.Setup(p => p.GetAll()).Returns(EmptyGamesList());

            var service = new GameService(mapper!, achievementRepo.Object, gameRepo.Object, context.Object);
            var actualServiceResponse = await service.GetAllGames();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
            Assert.AreEqual("No games found.", actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task GetAllGames_RepoUnavailable_FailedServiceResponse()
        {
            gameRepo.Setup(p => p.GetAll()).Throws(new Exception("Test message"));

            var service = new GameService(mapper!, achievementRepo.Object, gameRepo.Object, context.Object);
            var actualServiceResponse = await service.GetAllGames();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.AreEqual("Test message", actualServiceResponse.Message);
            Assert.IsFalse(actualServiceResponse.Success);
        }

        private async static Task<IEnumerable<Game?>> Seed()
        {
            var games = new List<Game?>
            {
                new Game
                {
                    Id = new Guid("6a3dbb1c-0001-41c4-9e84-410f17b644e7"),
                    Name = "Game 1",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    LastUpdated = DateTime.UtcNow
                },
                new Game
                {
                    Id = new Guid("6a3dbb1c-0002-41c4-9e84-410f17b644e7"),
                    Name = "Game 2",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    LastUpdated = DateTime.UtcNow
                },
                new Game
                {
                    Id = new Guid("6a3dbb1c-0003-41c4-9e84-410f17b644e7"),
                    Name = "Game 3",
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = true,
                    LastUpdated = DateTime.UtcNow
                }
            };

            return games;
        }

        private async static Task<IEnumerable<Game?>> EmptyGamesList()
        {
            var games = new List<Game?>();
            return games.AsEnumerable();
        }
    }
}
