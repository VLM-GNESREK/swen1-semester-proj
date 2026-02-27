// DTO/UserLeaderBoardEntryDTO.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.DTO
{
    public class UserLeaderBoardEntryDTO
    {

        // ## PROPERTIES ##

        public int UserID { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }

        // ## CONSTRUCTOR ##

        public UserLeaderBoardEntryDTO(User user, int rating)
        {
            UserID = user.UserID;
            Username = user.Username;
            Rating = rating;
        }
    }
}