// Services/RatingService.cs

using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;

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
            int newID = _repo.CreateRating(user, media, stars, comment);
            Rating newRating = new Rating(newID, user, media, stars, comment);
            return newRating;
        }
    }
}