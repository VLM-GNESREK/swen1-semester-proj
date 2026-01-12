// Services/MediaService.cs

using System.Data.SqlTypes;
using Npgsql;
using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Services
{
    public class MediaService
    {

        // ## VARIABLES ##

        IMediaRepository _mediaRepository;

        // ## METHODS ##

        public MediaService(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public MediaEntry CreateMedia(string title, string description, int releaseYear, MediaType type, List<string> genres, int ageRestriction, User user)
        {
            int newID = _mediaRepository.CreateMedia
                                                    (
                                                        title, 
                                                        description, 
                                                        releaseYear, 
                                                        type, 
                                                        genres,
                                                        ageRestriction,
                                                        user.UserID
                                                    );
            MediaEntry newMedia = new MediaEntry(newID, title, description, releaseYear, user);
            newMedia.Type = type;
            newMedia.Genres = genres;
            newMedia.AgeRestriction = ageRestriction;
            
            return newMedia;
        }

        public MediaEntry? FindMedia(int mediaID)
        {
            return _mediaRepository.GetMediaByID(mediaID);
        }

        public void DeleteMedia(int mediaID)
        {
            _mediaRepository.DeleteMedia(mediaID);
        }

        public void UpdateMedia(MediaEntry media)
        {
            _mediaRepository.UpdateMedia(media);
        }

        public List<MediaEntry> GetAllMedia()
        {
            List<MediaEntry> mediaList = _mediaRepository.GetAllMedia();
            return mediaList;
        }

        public List<MediaEntry> GetFilteredMedia(string? title, string? type, string? genre)
        {
            List<MediaEntry> mediaList = _mediaRepository.GetFilteredMedia(title, type, genre);
            return mediaList;
        }
    }
}