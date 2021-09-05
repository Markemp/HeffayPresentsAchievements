using AutoMapper;
using HeffayPresentsAchievements;
using HeffayPresentsAchievements.Models;
using HeffayPresentsAchievements.Models.Dtos.Game;
using HeffayPresentsAchievements.Services.AchievementService;
using HeffayPresentsAchievements.Services.GameService;
using HeffayPresentsAchievements.Services.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeffayPresentsAchievementsTests.UnitTests.ServicesTests
{
    [TestClass]
    public class GameServiceTests
    {
        private IMapper? mapper;
        private readonly Mock<IAchievementService> achievementService = new();
        private readonly Mock<IRepository<Game>> gameRepo = new();
        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor = new();
        private Guid badGuid = new("baddbb1c-7b7b-41c4-9e84-410f17b64bad");
        private Guid testUserId = new("e5e6095b-0aba-4b78-f99e-08d964b8ccd4");
        private readonly DefaultHttpContext context = new();

        [TestInitialize]
        public void Initialize()
        {
            var profile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            mapper = new Mapper(configuration);
            ClaimsPrincipal user = new();
            var claimsIdentity = new ClaimsIdentity();
            var claim = new Claim(ClaimTypes.NameIdentifier, testUserId.ToString());
            claimsIdentity.AddClaim(claim);
            user.AddIdentity(claimsIdentity);
            context.User = user;
        }

        [TestMethod]
        public async Task GetAllGames_ShouldReturnAllGamesForUser()
        {
            gameRepo.Setup(p => p.GetAllForId(It.IsAny<Guid>())).Returns(SeedGames()!);
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var actualServiceResponse = await service.GetAllGames();

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.AreEqual(2, actualServiceResponse.Data.Count);
            Assert.AreEqual("Game 1", actualServiceResponse.Data[0].Name);
            Assert.AreEqual(new Guid("6a3dbb1c-0001-41c4-9e84-410f17b644e7"), actualServiceResponse.Data[0].Id);
        }

        [TestMethod]
        public async Task GetAllGames_NoGamesReturned()
        {
            gameRepo.Setup(p => p.GetAllForId(It.IsAny<Guid>())).Returns(EmptyGamesList());
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var actualServiceResponse = await service.GetAllGames();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
            Assert.AreEqual("No games found.", actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task GetAllGames_RepoUnavailable_FailedServiceResponse()
        {
            gameRepo.Setup(p => p.GetAllForId(It.IsAny<Guid>())).Throws(new Exception("Test message"));
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var actualServiceResponse = await service.GetAllGames();

            Assert.IsNull(actualServiceResponse.Data);
            Assert.AreEqual("Test message", actualServiceResponse.Message);
            Assert.IsFalse(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetGameById_Successful()
        {
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(SeedGames().Result.FirstOrDefault()));
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);

            var actualServiceResponse = await service.GetGameById(new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7"));

            Assert.IsNotNull(actualServiceResponse.Data);
            Assert.IsTrue(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task GetGameById_GameNotFound_SuccessFalseAndNotFoundMessage()
        {
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult<Game?>(null));

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);

            var actualServiceResponse = await service.GetGameById(badGuid);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals($"Game {badGuid} not found."));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task GetAchievementById_RepoUnavailable_FailedServiceResponse()
        {
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Throws(new Exception("Test message"));

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var actualServiceResponse = await service.GetGameById(badGuid);

            Assert.IsNull(actualServiceResponse.Data);
            Assert.AreEqual("Test message", actualServiceResponse.Message);
            Assert.IsFalse(actualServiceResponse.Success);
        }

        [TestMethod]
        public async Task AddGame_Success()
        {
            gameRepo.Setup(p => p.Add(It.IsAny<Game>())).Returns(Task.FromResult(1));
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(SeedGames().Result.FirstOrDefault()));

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);

            var newGame = new AddGameDto("New Game", new Guid());

            var actualServiceResponse = await service.AddGame(newGame);

            Assert.AreEqual("Game 1", actualServiceResponse.Data!.Name);
            Assert.AreEqual("Added 1 row (should be 1).", actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task AddGame_NullSubmitted_ThrowNullArgException()
        {
            gameRepo.Setup(p => p.Add(It.IsAny<Game>())).Throws(new ArgumentNullException());

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            AddGameDto? game = null;

            var actualServiceResponse = await service.AddGame(game!);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.AreEqual("Object reference not set to an instance of an object.", actualServiceResponse.Message);
        }

        [TestMethod]
        public async Task AddGame_GameSubmitted_ThrowException()
        {
            gameRepo.Setup(p => p.Add(It.IsAny<Game>())).Throws(new Exception());

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            AddGameDto game = new("New Game", new Guid());

            var actualServiceResponse = await service.AddGame(game);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.StartsWith("Exception of type "));
        }

        [TestMethod]
        public async Task UpdateAchievement_Success()
        {
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(SeedGames().Result.FirstOrDefault()));
            gameRepo.Setup(p => p.Update(It.IsAny<Game>())).Returns(Task.FromResult(new Game()));

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            UpdateGameDto updatedGame = new(new Guid(), "updated game name");

            var actualServiceResponse = await service.UpdateGame(updatedGame);

            Assert.IsTrue(actualServiceResponse.Success);
            Assert.IsNull(actualServiceResponse.Message);
            Assert.IsNotNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task UpdateGame_GameNotFound()
        {
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult<Game?>(null));

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            UpdateGameDto updatedGame = new(new Guid(), "updated game name");

            var actualServiceResponse = await service.UpdateGame(updatedGame);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.StartsWith("Game "));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task UpdateGame_UpdateFailed()
        {
            gameRepo.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(SeedGames().Result.FirstOrDefault()));
            gameRepo.Setup(p => p.Update(It.IsAny<Game>())).Throws(new Exception());

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            UpdateGameDto updatedGame = new(new Guid(), "updated game name");

            var actualServiceResponse = await service.UpdateGame(updatedGame);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Exception of type 'System.Exception' was thrown."));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task DeleteGame_Success()
        {
            gameRepo.Setup(p => p.Remove(It.IsAny<Guid>())).Returns(Task.FromResult(1));
            gameRepo.Setup(p => p.GetAll()).Returns(SeedGames());

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var idToDelete = new Guid("6a3dbb1c-7b7b-41c4-9e84-410f17b644e7");

            var actualServiceResponse = await service.DeleteGame(idToDelete);

            Assert.IsTrue(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Removed 1 rows."));
            Assert.AreEqual(2, actualServiceResponse.Data!.Count);
        }

        [TestMethod]
        public async Task DeleteAchievement_AchievementNotFound()
        {
            gameRepo.Setup(p => p.Remove(It.IsAny<Guid>())).Returns(Task.FromResult(0));
            gameRepo.Setup(p => p.GetAll()).Returns(SeedGames());

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var idToDelete = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

            var actualServiceResponse = await service.DeleteGame(idToDelete);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Game not found."));
            Assert.IsNull(actualServiceResponse.Data);
        }

        [TestMethod]
        public async Task DeleteAchievement_ExceptionThrown()
        {
            gameRepo.Setup(p => p.Remove(It.IsAny<Guid>())).Throws(new Exception());
            gameRepo.Setup(p => p.GetAll()).Returns(SeedGames());

            var service = new GameService(mapper!, achievementService.Object, gameRepo.Object, mockHttpContextAccessor.Object);
            var idToDelete = new Guid("baddbb1c-7b7b-41c4-9e84-410f17b64bad");

            var actualServiceResponse = await service.DeleteGame(idToDelete);

            Assert.IsFalse(actualServiceResponse.Success);
            Assert.IsTrue(actualServiceResponse.Message!.Equals("Exception of type 'System.Exception' was thrown."));
            Assert.IsNull(actualServiceResponse.Data);
        }


        private async static Task<IEnumerable<Game?>> SeedGames()
        {
            var games = await Task.Run(() => new List<Game?>
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
            });

            return games;
        }

        private async static Task<IEnumerable<User?>> SeedUsers()
        {
            var users = await Task.Run(() => new List<User?>
            {
                new User
                {
                    Id = new Guid("e5e6095b-0aba-4b78-f99e-08d964b80001"),
                    Email = "test1@example.com",
                    Username = "test1",
                    PasswordHash = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 },
                    PasswordSalt = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }
                },
                new User
                {
                    Id = new Guid("e5e6095b-0aba-4b78-f99e-08d964b80002"),
                    Email = "test2@example.com",
                    Username = "test2",
                    PasswordHash = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 },
                    PasswordSalt = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }
                },
                new User
                {
                    Id = new Guid("e5e6095b-0aba-4b78-f99e-08d964b80003"),
                    Email = "test3@example.com",
                    Username = "test3",
                    PasswordHash = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 },
                    PasswordSalt = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }
                },
            });

            return users;
        }

        private async static Task<IEnumerable<Game?>> EmptyGamesList()
        {
            var games = await Task.Run(() => new List<Game?>());
            return games.AsEnumerable();
        }
    }
}
