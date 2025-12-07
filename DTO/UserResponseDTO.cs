// DTO/UserResponseDTO.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.DTO
{
    public class UserResponseDTO
    {
        public string Username { get; set; }
        public int UserID { get; set; }

        public UserResponseDTO(User user)
        {
            this.Username = user.Username;
            this.UserID = user.UserID;
        }
    }
}