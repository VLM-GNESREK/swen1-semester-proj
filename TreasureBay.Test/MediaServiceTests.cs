// TreasureBay.Test/MediaServiceTests.cs

using Treasure_Bay.Classes;
using Treasure_Bay.Services;
using NUnit.Framework;
using System.Collections.Generic;

namespace Treasure_Bay.Tests
{
    public class MediaServiceTests
    {
        private FakeMediaRepository _fakeRepo;
        private MediaService _mediaService;
        private User _dummyUser;

        [SetUp]
        public void SetUp()
        {
            _fakeRepo = new FakeMediaRepository();
            _mediaService = new MediaService(_fakeRepo);
            _dummyUser = new User("Creator", 1, "hash");
        }

        [Test]
        public void CreateMedia_ShouldReturnObject_WhenValid()
        {
            MediaEntry media = _mediaService.CreateMedia("Inception", "Dream movie", 2010, _dummyUser);

            Assert.That(media, Is.Not.Null);
            Assert.That(media.Title, Is.EqualTo("Inception"));
            Assert.That(media.MediaID, Is.GreaterThan(0));
        }

        [Test]
        public void FindMedia_ShouldReturnMedia_WhenMediaExists()
        {
            var created = _mediaService.CreateMedia("Matrix", "Sci-Fi", 1999, _dummyUser);
            var found = _mediaService.FindMedia(created.MediaID);

            Assert.That(found, Is.Not.Null);
            Assert.That(found.Title, Is.EqualTo("Matrix"));
        }

        [Test]
        public void FindMedia_ShouldReturnNull_WhenMediaDoesNotExist()
        {
            var found = _mediaService.FindMedia(999);

            Assert.That(found, Is.Null);
        }

        [Test]
        public void GetAllMedia_ShouldReturnAllEntries()
        {
            _mediaService.CreateMedia("Movie1", "Desc", 2000, _dummyUser);
            _mediaService.CreateMedia("Movie2", "Desc", 2001, _dummyUser);

            List<MediaEntry> all = _mediaService.GetAllMedia();

            Assert.That(all.Count, Is.EqualTo(2));
            Assert.That(all[1].Title, Is.EqualTo("Movie2"));
        }

        [Test]
        public void UpdateMedia_ShouldChangeTitle_WhenCalled()
        {
            var media = _mediaService.CreateMedia("Old Title", "Desc", 2000, _dummyUser);
            media.Title = "New Title";
            _mediaService.UpdateMedia(media);

            var updated = _mediaService.FindMedia(media.MediaID);
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.Title, Is.EqualTo("New Title"));
        }

        [Test]
        public void DeleteMedia_ShouldRemoveEntry_WhenCalled()
        {
            var media = _mediaService.CreateMedia("Delete Me", "Desc", 2000, _dummyUser);

            _mediaService.DeleteMedia(media.MediaID);

            var found = _mediaService.FindMedia(media.MediaID);
            Assert.That(found, Is.Null);
        }

        [Test]
        public void DeleteMedia_ShouldDoNothing_WhenIDDoesNotExist()
        {
            Assert.DoesNotThrow(() => _mediaService.DeleteMedia(9999));
        }
    }
}