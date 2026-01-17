// TreasureBay.Test/FakeUserRepository.cs


using System.Collections.Generic;
using System.Linq;
using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Tests
{
    public class FakeUserRepository : IUserRepository
    {
        private List<User> _fakeDB = new List<User>();
        private int _nextID = 1;

        public int CreateUser(string username, string passwordHash)
        {
            int id = _nextID++;
            var user = new User(username, id, passwordHash);
            _fakeDB.Add(user);
            return id;
        }

        public User? GetUserByUsername(string username)
        {
            return _fakeDB.FirstOrDefault(u => u.Username == username);
        }

        public User? GetUserByID(int userID)
        {
            return _fakeDB.FirstOrDefault(u => u.UserID  == userID);
        }

        public void AddFavourite(int userID, int mediaID)
        {
            var user = GetUserByID(userID);
            if(user != null)
            {
                if(!user.Favourites.Any(m => m.MediaID == mediaID))
                {
                    User dummyCreator = new User("DummyCreator", 999, "hash");
                    MediaEntry dummyMedia = new MediaEntry(mediaID, "Test Media" + mediaID, "Desc", 2024, dummyCreator);

                    user.Favourites.Add(dummyMedia);
                }
            }
        }

        public void RemoveFavourite(int userID, int mediaID)
        {
            var user = GetUserByID(userID);
            if(user != null)
            {
                var mediaToRemove = user.Favourites.FirstOrDefault(m => m.MediaID == mediaID);
                if(mediaToRemove != null)
                {
                    user.Favourites.Remove(mediaToRemove);
                }
            }
        }

        public List<MediaEntry> GetFavourites(int userID)
        {
            var user = GetUserByID(userID);
            return user != null ? user.Favourites : new List<MediaEntry>();
        }
    }
}