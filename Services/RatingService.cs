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

        public List<MediaEntry> GetTopRatedMedia(List<MediaEntry> mediaList, int count)
        {
            return mediaList
                            .OrderByDescending(m => m.AverageRating)
                            .Take(count)
                            .ToList();          
        }

        public List<Rating> GetRatingsByMedia(MediaEntry media)
        {
            List<Rating> ratings = _repo.GetRatingsByMediaID(media);
            foreach(var rating in ratings)
            {
                if(!rating.ComVis)
                {
                    rating.Comment = "[Private Comment]";
                }
            }
            return ratings;
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

        public void LikeRating(int ratingID, int userID)
        {
            if(_repo.GetRatingByID(ratingID) == null)
            {
                throw new KeyNotFoundException("Rating not found.");
            }

            _repo.AddLike(ratingID, userID);
        }

        public void UnlikeRating(int ratingID, int userID)
        {
            if(_repo.GetRatingByID(ratingID) == null)
            {
                throw new KeyNotFoundException("Rating not found.");
            }

            _repo.RemoveLike(ratingID, userID);
        }

        public void SetCommentVisibility(int ratingID, bool isVisible)
        {
            if(_repo.GetRatingByID(ratingID) == null)
            {
                throw new KeyNotFoundException("Rating not found.");
            }

            _repo.SetCommentVisibility(ratingID, isVisible);
        }
    }
}
