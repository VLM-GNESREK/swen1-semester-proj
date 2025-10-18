// Classes/MediaEntry.cs

using System.Collections.Generic;

namespace Treasure_Bay.Classes
{
    public enum MediaType
    {
        Movie,
        Series,
        Game
    }
    public class MediaEntry
    {
        // ## PROPERTIES ##
        // Normal props
        public int MediaID { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int AgeRestriction { get; set; }
        // Shiny types (props)
        public User Creator { get; private set; }
        public List<Rating> Ratings { get; private set; }
        public List<string> Genres { get; set; }
        public MediaType Type { get; set; }

        // ## METHODS ##
        // Constructor

        public MediaEntry(int mediaID, string title, string description, int releaseYear, User creator)
        {
            this.MediaID = mediaID;
            this.Title = title;
            this.Description = description;
            this.ReleaseYear = releaseYear;
            this.Creator = creator;
            this.Genres = new List<string>();
            this.Ratings = new List<Rating>();
        }

        // Rating Stuff

        public void AddRating(Rating rating)
        {
            this.Ratings.Add(rating);
        }

        // Genre Stuff

        public void AddGenre(string genre)
        {
            if (!string.IsNullOrWhiteSpace(genre) && !this.Genres.Contains(genre)) // Checks if genre submitted is Null or Empty, and also other misc. chars e.g. \n \r etc. (Very handy)
            {
                this.Genres.Add(genre);
            }
        }
    }
}