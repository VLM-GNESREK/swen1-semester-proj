// Repositories/IMediaRepository.cs

using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public interface IMediaRepository
    {
        int CreateMedia(string title, string description, int releaseYear, int userID);
        MediaEntry? GetMediaByID(int id);
        List<MediaEntry> GetAllMedia();
        void UpdateMedia(MediaEntry media);
        void DeleteMedia(int id);
    }
}