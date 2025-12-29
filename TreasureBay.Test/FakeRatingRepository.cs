// TreasureBay.Test/FakeRatingRepository.cs

using System.Collections.Generic;
using System.Linq;
using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Tests
{
    public class FakeRatingRepository : IRatingRepository
    {

        // ## VARIABLES ## 
        private List<Rating> _fakeRatingDB = new List<Rating>();
        private int _nextID = 1;

        // ## METHODS ##

        public int CreateRating(User reviewer, MediaEntry media, int stars, string comment)
        {
            Rating rating = new Rating(_nextID, reviewer, media, stars, comment);
            _fakeRatingDB.Add(rating);
            _nextID++;
            return rating.RatingID;
        }

        public List<Rating> GetRatingsByMediaID(MediaEntry media)
        {
            return _fakeRatingDB.FindAll(u => u.Media == media);
        }

        public List<Rating> GetRatingsByUserID(User reviewer)
        {
            return _fakeRatingDB.FindAll(u => u.Reviewer == reviewer);
        }

        public void UpdateRating(Rating rating)
        {
            int index = _fakeRatingDB.FindIndex(u => u.RatingID == rating.RatingID);
            _fakeRatingDB[index] = rating;
        }

        public void DeleteRating(Rating rating)
        {
            _fakeRatingDB.Remove(rating);
        }
    }
}