// Repositories/IUserRepository.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByUsername(string username);
        User? GetUserByID(int id);
        int CreateUser(string username, string hashedPassword);
        void AddFavourite(int userID, int mediaID);
        void RemoveFavourite(int userID, int mediaID);
        List<MediaEntry> GetFavourites(int userID);
    }
}