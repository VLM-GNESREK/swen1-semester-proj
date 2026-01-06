// TreasureBay.Test/AuthServiceTests.cs

using Treasure_Bay.Services;
using NUnit.Framework;

namespace Treasure_Bay.Tests
{
    public class AuthServiceTests
    {
        private AuthService _authService;

        [SetUp]
        public void SetUp()
        {
            _authService = new AuthService();
        }

        [Test]
        public void HashPassword_ShouldReturnADifferentString_WhenCalled()
        {
            string password = "SecretPassword123!";
            string hash = _authService.HashPassword(password);

            Assert.That(hash, Is.Not.EqualTo(password));
            Assert.That(hash, Is.Not.Empty);
        }

        [Test]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            string password = "MyPassword";
            string hash = _authService.HashPassword(password);

            bool result = _authService.VerifyPassword(password, hash);

            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsWrong()
        {
            string password = "MyPassword";
            string hash = _authService.HashPassword(password);

            bool result = _authService.VerifyPassword("WrongPassword", hash);

            Assert.That(result, Is.False);
        }
    }
}