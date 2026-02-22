// Repositories/IMediaRepository.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public interface IMediaRepository
    {
        int CreateMedia(string title, string description, int releaseYear, MediaType type, List<string> genres, int ageRestriction, int userID);
        MediaEntry? GetMediaByID(int id);
        List<MediaEntry> GetAllMedia();
        List<MediaEntry> GetFilteredMedia(string? title, string? type, string? genre);
        void UpdateMedia(MediaEntry media);
        void DeleteMedia(int id);
        void AddFavourite(int userID, int mediaID);
        void RemoveFavourite(int userID, int mediaID);
        List<MediaEntry> GetFavouritesByUserID(int userID);
    }
}