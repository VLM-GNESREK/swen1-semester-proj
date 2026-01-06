// TreasureBay.Test/DTOTests.cs

using Treasure_Bay.DTO;
using NUnit.Framework;
using Treasure_Bay.Classes;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Treasure_Bay.Tests
{
    public class DTOTests
    {
        [Test]
        public void UserResponseDTO_ShouldMapFieldsCorrectly()
        {
            User user = new User("TestUser", 55, "SuperSecretHash");

            UserResponseDTO dto = new UserResponseDTO(user);

            Assert.That(dto.Username, Is.EqualTo("TestUser"));
            Assert.That(dto.UserID, Is.EqualTo(55));
        }

        [Test]
        public void MediaResponseDTO_ShouldMapFieldsCorrectly()
        {
            User creator = new User("Director", 1, "Hash123");
            MediaEntry media = new MediaEntry(10, "Movie", "Desc", 2022, creator);

            MediaResponseDTO dto = new MediaResponseDTO(media);

            Assert.That(dto.Creator, Is.Not.Null);
            Assert.That(dto.Creator, Is.InstanceOf<UserResponseDTO>());
            Assert.That(dto.Creator.Username, Is.EqualTo("Director"));
            Assert.That(dto.Title, Is.EqualTo("Movie"));
        }
    }
}
