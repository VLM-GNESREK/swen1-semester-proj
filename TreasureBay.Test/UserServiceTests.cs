// TreasureBay.Test/UserServiceTests.cs

using NUnit.Framework;
using Treasure_Bay.Services;
using Treasure_Bay.DTO;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Tests
{
    public class UserServiceTests
    {
        private UserService _userService;
        private FakeUserRepository _fakeRepo;

        [SetUp]
        public void Setup()
        {
            _fakeRepo = new FakeUserRepository();
            _userService = new UserService(_fakeRepo);
        }

        [Test]
        public void Register_ShouldCreateUser_WhenDataIsValid()
        {
            string username = "TestUser";
            string password = "password123";

            UserResponseDTO result = _userService.Register(username, password);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.UserID, Is.GreaterThan(0));
            Assert.That(_fakeRepo.GetUserByUsername(username), Is.Not.Null);
        }

        [Test]
        public void Login_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            string username = "LoginUser";
            string password = "securePassword";
            _userService.Register(username, password);

            User? loggedInUser = _userService.Login(username, password);

            Assert.That(loggedInUser, Is.Not.Null);
            Assert.That(loggedInUser.Username, Is.EqualTo(username));
        }

        [Test]
        public void Login_ShouldFail_WhenPasswordIsWrong()
        {
            string username = "Hacker";
            _userService.Register(username, "correctPassword");

            User? result = _userService.Login(username, "wrongPassword");

            Assert.That(result, Is.Null, "Login sollte fehlschlagen");
        }
    }
}