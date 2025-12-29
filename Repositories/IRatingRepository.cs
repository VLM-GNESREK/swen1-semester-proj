// Repositories/IRatingRepository.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public interface IRatingRepository
    {
        int CreateRating(User reviewer, MediaEntry media, int stars, string comment);
        List<Rating> GetRatingsByMediaID(MediaEntry media);
        List<Rating> GetRatingsByUserID(User reviewer);
        void UpdateRating(Rating rating);
        void DeleteRating(Rating rating);
    }
}