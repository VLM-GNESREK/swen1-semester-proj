// Services/UserService.cs

using System.Linq.Expressions;
using Treasure_Bay.Classes;
using Treasure_Bay.DTO;

namespace Treasure_Bay.Services
{
    public class UserService
    {
        
        // ## COLLECTIONS ##

        private static List<User> userDatabase = new List<User>();

        // ## METHODS ##

        public bool QueryExists(string Username)
        {
            return userDatabase.Any(u => u.Username == Username);
        }

        public User? Login(string Username, string Password)
        {
            User? user = userDatabase.FirstOrDefault(u => u.Username == Username);
            if(user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public UserResponseDTO Register(string Username, string Password)
        {
            int newID = userDatabase.Any() ? userDatabase.Max(u => u.UserID) + 1 : 1;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
            User newUser = new User(Username, newID, hashedPassword);
            userDatabase.Add(newUser);
            UserResponseDTO userResponse = new UserResponseDTO(newUser);
            return userResponse;
        }
    }
}