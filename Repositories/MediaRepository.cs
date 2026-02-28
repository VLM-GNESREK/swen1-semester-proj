// Repositories/MediaRepository.cs

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Npgsql;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        public int CreateMedia(string title, string description, int releaseYear, MediaType type, List<string> genres, int ageRestriction, int userID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "INSERT INTO media (title, description, release_year, media_type, genre, age_restriction, user_id) VALUES (@t, @d, @r, @mt, @g, @a, @u) RETURNING media_id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", title);
                    cmd.Parameters.AddWithValue("d", description);
                    cmd.Parameters.AddWithValue("r", releaseYear);
                    cmd.Parameters.AddWithValue("mt", type.ToString()); // Enum -> String
                    cmd.Parameters.AddWithValue("g", string.Join(",", genres)); // List -> "Action,Comedy"
                    cmd.Parameters.AddWithValue("a", ageRestriction);
                    cmd.Parameters.AddWithValue("u", userID);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public MediaEntry? GetMediaByID(int id)
        {
            MediaEntry? returnMedia = null;
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"SELECT 
                                m.title, m.description, m.release_year, m.user_id, m.media_type, m.genre, m.age_restriction,
                                u.username, u.password_hash,
                                COALESCE((SELECT AVG(star_value) FROM ratings WHERE media_id = m.media_id), 0)::float8 AS avg_rating
                            FROM media m 
                            JOIN users u ON m.user_id = u.user_id
                            WHERE m.media_id = @m";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m", id);

                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            string title = reader.GetString(0);
                            string desc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            int releaseYear = reader.GetInt32(2);
                            int userID = reader.GetInt32(3);
                            string mediaType = reader.IsDBNull(4) ? "Unknown" : reader.GetString(4);
                            string genre = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            int ageRestriction = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                            
                            string username = reader.GetString(7);
                            string dbHash = reader.GetString(8);
                            double avgRating = reader.GetDouble(9);

                            User user = new User(username, userID, dbHash);

                            MediaEntry foundMedia = new MediaEntry(id, title, desc, releaseYear, user);
                            foundMedia.AverageRating = avgRating;
                            foundMedia.Type = Enum.Parse<MediaType>(mediaType);
                            foundMedia.Genres = string.IsNullOrEmpty(genre) ? new List<string>() : genre.Split(',').ToList();
                            foundMedia.AgeRestriction = ageRestriction;
                            returnMedia = foundMedia;
                        }
                    }
                }
            }
            return returnMedia;
        }

        public void DeleteMedia(int id)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "DELETE FROM media WHERE media_id = @m";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMedia(MediaEntry media)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "UPDATE media SET title = @t, description = @d, release_year = @r WHERE media_id = @m";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", media.Title);
                    cmd.Parameters.AddWithValue("d", media.Description);
                    cmd.Parameters.AddWithValue("r", media.ReleaseYear);
                    cmd.Parameters.AddWithValue("m", media.MediaID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<MediaEntry> GetAllMedia()
        {
            List<MediaEntry> mediaList = new List<MediaEntry>();

            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"SELECT 
                                m.media_id, m.title, m.description, m.release_year, m.user_id, m.media_type, m.genre, m.age_restriction,
                                u.username, u.password_hash,
                                COALESCE((SELECT AVG(star_value) FROM ratings WHERE media_id = m.media_id), 0)::float8 AS avg_rating
                            FROM media m
                            JOIN users u ON m.user_id = u.user_id";
                
                using(var cmd = new NpgsqlCommand(sql, conn))
                using(var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        int userID = reader.GetInt32(4);
                        string mediaType = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5);
                        string genre = reader.IsDBNull(6) ? "" : reader.GetString(6);
                        int ageRestriction = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                        
                        string username = reader.GetString(8);
                        string dbHash = reader.GetString(9);
                        double avgRating = reader.GetDouble(10);

                        User user = new User(username, userID, dbHash);

                        int mediaID = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        int releaseYear = reader.GetInt32(3);

                        MediaEntry media = new MediaEntry(mediaID, title, desc, releaseYear, user);
                        media.AverageRating = avgRating;
                        media.Type = Enum.Parse<MediaType>(mediaType);
                        media.Genres = string.IsNullOrEmpty(genre) ? new List<string>() : genre.Split(',').ToList();
                        media.AgeRestriction = ageRestriction;
                        mediaList.Add(media);
                    }
                }
            }

            return mediaList;
        }

        public List<MediaEntry> GetFilteredMedia(string? title, string? type, string? genre)
        {
            List<MediaEntry> mediaList = new List<MediaEntry>();

            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sqlBuilder = new StringBuilder(@"SELECT
                                                        m.media_id, m.title, m.description, m.release_year, m.user_id, m.media_type, m.genre, m.age_restriction,
                                                        u.username, u.password_hash,
                                                        COALESCE((SELECT AVG(star_value) FROM ratings WHERE media_id = m.media_id), 0)::float8 AS avg_rating
                                                    FROM media m
                                                    JOIN users u ON m.user_id = u.user_id
                                                    WHERE 1=1");
                if(!string.IsNullOrEmpty(title))
                {
                    sqlBuilder.Append(" AND m.title ILIKE @title");
                }

                if(!string.IsNullOrEmpty(type))
                {
                    sqlBuilder.Append(" AND m.media_type = @type");
                }

                if(!string.IsNullOrEmpty(genre))
                {
                    sqlBuilder.Append(" AND m.genre = @genre");
                }

                using(var cmd = new NpgsqlCommand(sqlBuilder.ToString(), conn))
                {
                    if(!string.IsNullOrEmpty(title))
                    {
                        cmd.Parameters.AddWithValue("title", $"%{title}%");
                    }

                    if(!string.IsNullOrEmpty(type))
                    {
                        cmd.Parameters.AddWithValue("type", type);
                    }

                    if(!string.IsNullOrEmpty(genre))
                    {
                        cmd.Parameters.AddWithValue("genre", genre);
                    }

                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int userID = reader.GetInt32(4);
                            string mediaType = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5);
                            string genres = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            int ageRestriction = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                            string username = reader.GetString(8);
                            string dbHash = reader.GetString(9);
                            double avgRating = reader.GetDouble(10);

                            User user = new User(username, userID, dbHash);

                            int mediaID = reader.GetInt32(0);
                            string dbTitle = reader.GetString(1);
                            string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            int releaseYear = reader.GetInt32(3);

                            MediaEntry media = new MediaEntry(mediaID, dbTitle, desc, releaseYear, user);
                            media.AverageRating = avgRating;
                            media.Type = Enum.Parse<MediaType>(mediaType);
                            media.Genres = string.IsNullOrEmpty(genres) ? new List<string>() : genres.Split(',').ToList();
                            media.AgeRestriction = ageRestriction;
                            mediaList.Add(media);
                        }
                    }
                }
            }
            return mediaList;
        }
    }
}