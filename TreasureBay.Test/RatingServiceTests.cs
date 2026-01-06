// TreasureBay.Test/RatingServiceTests.cs

using System;
using System.Collections.Generic;
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

        [Test]
        public void GetAverageRating_ShouldReturnZero_WhenNoRatings()
        {
            User user = new User("User1", 1, "pw");
            MediaEntry media = new MediaEntry(10, "Unpopular Movie", "desc", 2020, user);

            double average = _ratingService.GetAverageRating(media);

            Assert.That(average, Is.EqualTo(0));
        }

        [Test]
        public void GetAverageRating_ShouldReturnCorrectAverage_WhenRatingsExist()
        {
            User u1 = new User("User1", 1, "pw");
            User u2 = new User("User2", 2, "pw");
            MediaEntry media = new MediaEntry(10, "Popular Movie", "desc", 2020, u1);

            _fakeRepo.CreateRating(u1, media, 5, "Loved it!");
            _fakeRepo.CreateRating(u2, media, 3, "It was okay.");

            double average = _ratingService.GetAverageRating(media);

            Assert.That(average, Is.EqualTo(4.0));
        }

        [Test]
        public void GetTopSortedMedia_ShouldReturnSortedTopList()
        {
            User testUser = new User("TestUser", 1, "hash");
            MediaEntry m1 = new MediaEntry(1, "Media1", "desc", 2004, testUser);
            MediaEntry m2 = new MediaEntry(2, "Media2", "desc", 2005, testUser);
            MediaEntry m3 = new MediaEntry(3, "Media3", "desc", 2006, testUser);

            _fakeRepo.CreateRating(testUser, m1, 5, "Excellent!");
            _fakeRepo.CreateRating(testUser, m2, 3, "Okay.");
            _fakeRepo.CreateRating(testUser, m3, 1, "Terrible!");

            List<MediaEntry> allMedia = new List<MediaEntry> { m3, m1, m2 };

            List<MediaEntry> sortedList = _ratingService.GetTopRatedMedia(allMedia, 3);

            Assert.That(sortedList[0], Is.EqualTo(m1));
            Assert.That(sortedList[0].Title, Is.EqualTo("Media1"));
            Assert.That(sortedList[2], Is.EqualTo(m3));
        }

        [Test]
        public void GetRatingsByMedia_ShouldReturnEmptyList_WhenNoRatingsExist()
        {
            User user = new User("User1", 1, "pw");
            MediaEntry media = new MediaEntry(50, "Quiet Movie", "shhh", 2023, user);

            List<Rating> results = _ratingService.GetRatingsByMedia(media);

            Assert.That(results, Is.Not.Null);
            Assert.That(results, Is.Empty);
        }
    }
}