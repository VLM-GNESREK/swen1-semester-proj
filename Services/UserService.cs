// Services/UserService.cs

using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using Npgsql;
using Treasure_Bay.Classes;
using Treasure_Bay.DTO;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Services
{
    public class UserService
    {

        // ## VARIABLES ##

        private readonly IUserRepository _userRepository;

        // ## METHODS ##

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool QueryUserExists(string _username)
        {
           return _userRepository.GetUserByUsername(_username) != null;
        }

        public User? Login(string _username, string _password)
        {
            User? user = _userRepository.GetUserByUsername(_username);

            if(user == null || !BCrypt.Net.BCrypt.Verify(_password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public UserResponseDTO Register(string _username, string Password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
            int newID = _userRepository.CreateUser(_username, hashedPassword);
            User newUser = new User(_username, newID, hashedPassword);
            return new UserResponseDTO(newUser);
        }
    }
}