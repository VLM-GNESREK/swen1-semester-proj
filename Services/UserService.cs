// Services/UserService.cs

using Treasure_Bay.Classes;
using Treasure_Bay.DTO;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Services
{
    public class UserService
    {

        // ## VARIABLES ##

        private readonly IUserRepository _userRepository;
        private AuthService _authService;

        // ## METHODS ##

        public UserService(IUserRepository userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public bool QueryUserExists(string _username)
        {
           return _userRepository.GetUserByUsername(_username) != null;
        }

        public User? Login(string _username, string _password)
        {
            User? user = _userRepository.GetUserByUsername(_username);

            if(user == null || !_authService.VerifyPassword(_password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public UserResponseDTO Register(string _username, string Password)
        {
            string hashedPassword = _authService.HashPassword(Password);
            int newID = _userRepository.CreateUser(_username, hashedPassword);
            User newUser = new User(_username, newID, hashedPassword);
            return new UserResponseDTO(newUser);
        }

        public User? GetUser(int userID)
        {
            return _userRepository.GetUserByID(userID);
        }

        public void RemoveFavourite(int userID, int mediaID)
        {
            _userRepository.RemoveFavourite(userID, mediaID);
        }

        public void AddFavourite(int userID, int mediaID)
        {
            _userRepository.AddFavourite(userID, mediaID);
        }

        public List<MediaResponseDTO> GetFavourites(int userID)
        {
            List<MediaEntry> favs = _userRepository.GetFavourites(userID);
            List<MediaResponseDTO> favDTOs = favs.Select(entry => new MediaResponseDTO(entry)).ToList();
            return favDTOs;
        }

        public UserProfileDTO? GetUserStatistics(int userID)
        {
            UserProfileDTO? userStats = _userRepository.GetUserStatistics(userID);

            if(userStats == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return userStats;
        }

        public List<UserLeaderBoardEntryDTO> GetLeaderBoard(int limit)
        {
            if(limit <= 0)
            {
                throw new ArgumentException("Limit must be greater than 0.");
            }

            if(limit > 100)
            {
                throw new ArgumentException("Limit cannot exceed 100.");
            }

            return _userRepository.LeaderBoard(limit);
        }
    }
}