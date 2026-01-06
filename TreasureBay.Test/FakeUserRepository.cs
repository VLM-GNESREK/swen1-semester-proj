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
    }
}