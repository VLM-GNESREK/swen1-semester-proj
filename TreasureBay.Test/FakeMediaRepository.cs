// Treasurebay.Test/FakeMediaRepository.cs

using System.Collections.Generic;
using System.Linq;
using Treasure_Bay.Classes;
using Treasure_Bay.Repositories;

namespace Treasure_Bay.Tests
{
    public class FakeMediaRepository : IMediaRepository
    {

        // ## VARIABLES ##

        private List<MediaEntry> _fakeDB = new List<MediaEntry>();
        private int _nextID = 1;

        // ## METHODS ##

        public int CreateMedia(string title, string description, int releaseYear, MediaType type, List<string> genres, int ageRestriction, int userID)
        {
            User fakeUser = new User("mockUser", userID, "hash");
            
            MediaEntry media = new MediaEntry(_nextID, title, description, releaseYear, fakeUser);
            
            media.Type = type;
            media.Genres = genres;
            media.AgeRestriction = ageRestriction;

            _fakeDB.Add(media);
            _nextID++;
            return media.MediaID;
        }

        public MediaEntry? GetMediaByID(int mediaID)
        {
            return _fakeDB.FirstOrDefault(m => m.MediaID == mediaID);
        }

        public List<MediaEntry> GetAllMedia()
        {
            return _fakeDB;
        }

        public void UpdateMedia(MediaEntry media)   
        {
            var existing = _fakeDB.FirstOrDefault(m => m.MediaID == media.MediaID);
            if (existing != null)
            {
                existing.Title = media.Title;
                existing.Description = media.Description;
                existing.ReleaseYear = media.ReleaseYear;
                existing.Type = media.Type;
                existing.Genres = media.Genres;
                existing.AgeRestriction = media.AgeRestriction;
            }
        }

        public void DeleteMedia(int mediaID)
        {
            var media = GetMediaByID(mediaID);
            if(media != null)
            {
                _fakeDB.Remove(media);
            }
        }

        public List<MediaEntry> GetFilteredMedia(string? title, string? type, string? genre)
        {
            IEnumerable<MediaEntry> query = _fakeDB;

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(m => m.Title.Contains(title, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(type))
            {
                if(System.Enum.TryParse<MediaType>(type, true, out MediaType parsedType))
                {
                    query = query.Where(m => m.Type == parsedType);
                }
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(m => m.Genres.Any(g => g.Equals(genre, System.StringComparison.OrdinalIgnoreCase)));
            }

            return query.ToList();
        }
    }
}