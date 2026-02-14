// Services/RatingService.cs

using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;
using System.Linq;

namespace Treasure_Bay.Services
{
    public class RatingService
    {
        
        // ## VARIABLES ##

        private IRatingRepository _repo;

        // ## METHODS ##

        public RatingService(IRatingRepository repo)
        {
            _repo = repo;
        }

        public Rating CreateRating(User user, MediaEntry media, int stars, string comment)
        {
            var list = _repo.GetRatingsByMediaID(media);
            if(list.FirstOrDefault(r => r.Reviewer.UserID == user.UserID) != null)
            {
                throw new InvalidOperationException("User has already rated this media.");
            }
            int newID = _repo.CreateRating(user, media, stars, comment);
            Rating newRating = new Rating(newID, user, media, stars, comment);
            return newRating;
        }

        public double GetAverageRating(MediaEntry media)
        {
            List<Rating> ratings = _repo.GetRatingsByMediaID(media);

            if(ratings.Count == 0)
            {
                return 0;
            }

            return ratings.Average(r => r.StarValue);
        }

        public List<MediaEntry> GetTopRatedMedia(List<MediaEntry> mediaList, int count)
        {
            return mediaList
                            .OrderByDescending(m => GetAverageRating(m))
                            .Take(count)
                            .ToList();          
        }

        public List<Rating> GetRatingsByMedia(MediaEntry media)
        {
            return _repo.GetRatingsByMediaID(media);
        }

        public void UpdateRating(User user, int ratingId, int stars, string comment)
        {
            Rating? rating = _repo.GetRatingByID(ratingId);

            if(rating == null)
            {
                throw new KeyNotFoundException("Rating not found.");
            }

            if(rating.Reviewer.UserID != user.UserID)
            {
                throw new UnauthorizedAccessException("You can only edit your own ratings.");
            }

            rating.StarValue = stars;
            rating.Comment = comment;
            _repo.UpdateRating(rating);
        }

        public void DeleteRating(int ratingID, User user)
        {
            Rating? rating = _repo.GetRatingByID(ratingID);

            if(rating == null)
            {
                throw new KeyNotFoundException("Rating not found.");
            }

            if(rating.Reviewer.UserID != user.UserID)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete this rating.");
            }

            _repo.DeleteRating(rating);
        }
    }
}
