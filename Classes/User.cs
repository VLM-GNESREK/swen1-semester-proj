// Classes/User.cs

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Treasure_Bay.Classes
{
    public class User
    {
        // ## PROPERTIES ##
        // Largely Similar to C++ (Featuring Properties (shiny))
        public string Username { get; set; }
        public int UserID { get; private set; }
        [JsonIgnore]
        public string PasswordHash { get; private set; }
        // Lists, basically C++ arrays. Live inside System.Collections.Generic or so StackOverflow says
        public List<Rating> Ratings { get; private set; }
        public List<MediaEntry> Favourites { get; private set; }

        // ## METHODS ##
        // Constructor

        public User(string Username, int UserID, string PasswordHash)
        {
            this.Username = Username;
            this.UserID = UserID;
            this.PasswordHash = PasswordHash;
            this.Ratings = []; // ref Rating.cs; List of Rating Objects
            this.Favourites = []; // ref MediaEntry.cs; List of MediaEntry Objects
        }

        // To be moved over to MediaController later:TM:

        public void PostRating(MediaEntry media, int stars, string comment)
        {
            int ratingId = this.Ratings.Count + 1; // Temp Fake ID, replace with UID once you figure out how
            Rating newRating = new Rating(ratingId, this, media, stars, comment);
            this.Ratings.Add(newRating);
            media.AddRating(newRating); // ref MediaEntry.cs
        }
    }
}