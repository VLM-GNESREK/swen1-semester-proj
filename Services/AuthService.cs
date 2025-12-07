// Services/AuthService.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Services
{
    public class AuthService
    {
        
        // ## COLLECTIONS ##

        private static Dictionary<string, User> tokenDatabase = new Dictionary<string, User>();

        // ## METHODS ##

        public string GenerateToken(User user)
        {
            string token = $"{user.Username}-mrpToken";
            tokenDatabase[token] = user;
            return token;
        }

        public User? GetUserByToken(string token)
        {
            if (tokenDatabase.TryGetValue(token, out User? user))
            {
                return user;
            }
            return null;
        }
    }
}