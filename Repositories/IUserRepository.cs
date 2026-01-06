// Repositories/IUserRepository.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByUsername(string username);
        User? GetUserByID(int id);
        int CreateUser(string username, string hashedPassword);
    }
}