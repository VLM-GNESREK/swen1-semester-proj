// TreasureBay.Test/RatingServiceTests.cs

using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using NUnit.Framework;
using Treasure_Bay.Classes;
using Treasure_Bay.Services;

namespace Treasure_Bay.Tests
{
    public class RatingServiceTests
    {
        private FakeRatingRepository _fakeRepo;
        private RatingService _ratingService;

        [SetUp]
        public void Setup()
        {
            _fakeRepo = new FakeRatingRepository();
            _ratingService = new RatingService(_fakeRepo);
        }

        [Test]
        public void CreateRating_ShouldAddRating_WhenValid()
        {
            User testUser = new User("TestUser", 1, "Hash123");
            MediaEntry testMedia = new MediaEntry(1, "Test Media", "This Media is just here for testing purposes.", 2004, testUser);

            Rating rating = _ratingService.CreateRating(testUser, testMedia, 5, "A great successful test!");

            Assert.That(rating, Is.Not.Null);
            Assert.That(rating.RatingID, Is.GreaterThan(0));
            Assert.That(rating.Media, Is.EqualTo(testMedia));
            Assert.That(rating.Reviewer, Is.EqualTo(testUser));
        }

        [Test]
        public void CreateRating_ShouldThrowException_WhenUserAlreadyRated()
        {
            User user = new User("SpamUser", 2, "hash");
            MediaEntry media = new MediaEntry(1, "Spam Target", "desc", 2020, user);

            _fakeRepo.CreateRating(user, media, 4, "First Rating!");

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ratingService.CreateRating(user, media, 1, "Spam Rating!");
            });

            Assert.That(ex.Message, Is.EqualTo("User has already rated this media."));
        }
    }
}