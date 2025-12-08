// Repositories/IUserRepository.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByUseranme(string username);
        int CreateUser(string username, string hashedPassword);
    }
}