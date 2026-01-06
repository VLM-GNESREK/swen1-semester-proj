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
        public UserResponseDTO? Creator { get; set; }

        // ## CONSTRUCTOR ##

        public MediaResponseDTO(MediaEntry media)
        {
            MediaID = media.MediaID;
            Title = media.Title;
            Description = media.Description;
            ReleaseYear = media.ReleaseYear;

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