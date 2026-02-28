// DTO/MediaResponseDTO.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.DTO
{
    public class MediaResponseDTO
    {

        // ## PROPERTIES ##

        public int MediaID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public string AverageRating { get; set; }
        public int AgeRestriction { get; set; }
        public List<string> Genres { get; set; }
        public string Type { get; set; }
        public UserResponseDTO? Creator { get; set; }

        // ## CONSTRUCTOR ##

        public MediaResponseDTO(MediaEntry media)
        {
            MediaID = media.MediaID;
            Title = media.Title;
            Description = media.Description;
            ReleaseYear = media.ReleaseYear;
            
            if(media.AverageRating == 0.0)
            {
                AverageRating = "Unrated";
            }
            else
            {
                AverageRating = media.AverageRating.ToString("0.0");
            }

            Type = media.Type.ToString();
            Genres = media.Genres;
            AgeRestriction = media.AgeRestriction;

            if(media.Creator != null)
            {
                Creator = new UserResponseDTO(media.Creator)
                {
                    Username = media.Creator.Username,
                    UserID = media.Creator.UserID
                };
            }
        }
    }
}