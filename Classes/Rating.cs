// Classes/Rating.cs

using System;

namespace Treasure_Bay.Classes
{
    public class Rating
    {
        // ## PROPERTIES ##
        // simple props
        public int RatingID { get; private set; }
        public int StarValue { get; set; }
        public string Comment { get; set; }
        public DateTime Timestamp { get; private set; }
        public int Likes { get; private set; }
        public bool ComVis { get; private set; }
        // connecty bits
        public User Reviewer { get; private set; }
        public MediaEntry Media { get; private set; }

        // ## PROPERTIES ##
        // Constructor

        public Rating(int ratingID, User reviewer, MediaEntry media, int starValue, string comment)
        {
            this.RatingID = ratingID;
            this.Reviewer = reviewer;
            this.Media = media;
            this.StarValue = starValue;
            this.Comment = comment;
            this.Timestamp = DateTime.Now;  // At time of creation
            this.Likes = 0;                 // No one likes you when you're created (Life philosophy right there)
            this.ComVis = false;            // Comment Visibility, moderation says Spec, not sure what they mean <-<
        }

        // All of this will be moved over to Controllers eventually:TM:

        public void ConfirmComment()
        {
            this.ComVis = true;
        }

        public void AddLike()
        {
            this.Likes++;
        }

        public void Removeike()
        {
            this.Likes--;
        }
    }
}