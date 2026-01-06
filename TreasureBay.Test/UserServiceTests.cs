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
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _fakeRepo = new FakeUserRepository();
            _authService = new AuthService();
            _userService = new UserService(_fakeRepo, _authService);
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
        public void Register_ShouldCreateTwoUsers_WhenUsernamesAreDifferent()
        {
            int id1 = _userService.Register("UserOne", "pass").UserID;
            int id2 = _userService.Register("UserTwo", "pass").UserID;
            Assert.That(id1, Is.Not.EqualTo(id2));
            Assert.That(id1, Is.GreaterThan(0));
            Assert.That(id2, Is.GreaterThan(0));
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

        [Test]
        public void QueryUserExists_ShouldBeTrue_WhenUserExists()
        {
            _userService.Register("TestUser", "testpass");

            bool answer = _userService.QueryUserExists("TestUser");

            Assert.That(answer, Is.True);
        }

        [Test]
        public void QueryUserExists_ShouldBeFalse_WhenUserDoesNotExist()
        {
            bool answer = _userService.QueryUserExists("GhostUser");

            Assert.That(answer, Is.False);
        }

        [Test]
        public void GetUser_ShouldReturnUser_WhenValid()
        {
            var response = _userService.Register("FoundUser", "Pass");
            int newID = response.UserID;

            User? user = _userService.GetUser(newID);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.UserID, Is.EqualTo(newID));
        }

        [Test]
        public void GetUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            User? user = _userService.GetUser(999);

            Assert.That(user, Is.Null);
        }
    }
}