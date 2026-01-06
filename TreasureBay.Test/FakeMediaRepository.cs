// Treasurebay.Test/FakeMediaRepository.cs

using System.Collections.Generic;
using System.Linq;
using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Tests
{
    public class FakeMediaRepository : IMediaRepository
    {

        // ## VARIABLES ##

        private List<MediaEntry> _fakeDB = new List<MediaEntry>();
        private int _nextID = 1;

        // ## METHODS ##

        public int CreateMedia(string title, string description, int release_year, int creator_id)
        {
            User dummyUser = new User("Dummy", creator_id, "hash");
            MediaEntry media = new MediaEntry(_nextID, title, description, release_year, dummyUser);
            _fakeDB.Add(media);
            _nextID++;
            return media.MediaID;
        }

        public MediaEntry? GetMediaByID(int mediaID)
        {
            return _fakeDB.FirstOrDefault(m => m.MediaID == mediaID);
        }

        public List<MediaEntry> GetAllMedia()
        {
            return _fakeDB;
        }

        public void UpdateMedia(MediaEntry media)
        {
            var existing = GetMediaByID(media.MediaID);
            if(existing != null)
            {
                _fakeDB.Remove(existing);
                _fakeDB.Add(media);
            }
        }

        public void DeleteMedia(int mediaID)
        {
            var media = GetMediaByID(mediaID);
            if(media != null)
            {
                _fakeDB.Remove(media);
            }
        }
    }
}