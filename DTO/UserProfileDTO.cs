// DTO/UserProfileDTO.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.DTO
{
    public class UserProfileDTO
    {
        public string Username { get; set; }
        public int UserID { get; set; }
        public int MediaCount { get; set; }
        public int FavouriteCount { get; set; }
        public int RatingCount { get; set; }

        public UserProfileDTO(User user, int mediaCount, int favouriteCount, int ratingCount)
        {
            this.Username = user.Username;
            this.UserID = user.UserID;
            this.MediaCount = mediaCount;
            this.FavouriteCount = favouriteCount;
            this.RatingCount = ratingCount;
        }
    }
}