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
                                m.title, m.description, m.release_year, m.user_id,
                                u.username, u.password_hash
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
                            string username = reader.GetString(4);
                            string dbHash = reader.GetString(5);

                            User user = new User(username, userID, dbHash);

                            MediaEntry foundMedia = new MediaEntry(id, title, desc, releaseYear, user);
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
                                m.media_id, m.title, m.description, m.release_year, m.user_id,
                                u.username, u.password_hash
                            FROM media m
                            JOIN users u ON m.user_id = u.user_id";
                
                using(var cmd = new NpgsqlCommand(sql, conn))
                using(var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        int userID = reader.GetInt32(4);
                        string username = reader.GetString(5);
                        string dbHash = reader.GetString(6);

                        User user = new User(username, userID, dbHash);

                        int mediaID = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        int releaseYear = reader.GetInt32(3);

                        MediaEntry media = new MediaEntry(mediaID, title, desc, releaseYear, user);

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
                                                        m.media_id, m.title, m.description, m.release_year, m.user_id,
                                                        u.username, u.password_hash
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
                            string username = reader.GetString(5);
                            string dbHash = reader.GetString(6);

                            User user = new User(username, userID, dbHash);

                            int mediaID = reader.GetInt32(0);
                            string dbTitle = reader.GetString(1);
                            string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            int releaseYear = reader.GetInt32(3);

                            MediaEntry media = new MediaEntry(mediaID, dbTitle, desc, releaseYear, user);

                            mediaList.Add(media);
                        }
                    }
                }
            }
            return mediaList;
        }

        public void AddFavourite(int userID, int mediaID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "INSERT INTO favourites (user_id, media_id) VALUES (@u, @m)";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", userID);
                    cmd.Parameters.AddWithValue("m", mediaID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveFavourite(int userID, int mediaID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "DELETE FROM favourites WHERE user_id = @u AND media_id = @m";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", userID);
                    cmd.Parameters.AddWithValue("m", mediaID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<MediaEntry> GetFavouritesByUserID(int userID)
        {
            List<MediaEntry> favourites = new List<MediaEntry>();

            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"SELECT 
                                m.media_id, m.title, m.description, m.release_year,
                                u.user_id AS creator_id, u.username, u.password_hash
                            FROM favourites f
                            JOIN media m ON f.media_id = m.media_id
                            JOIN users u ON m.user_id = u.user_id
                            WHERE f.user_id = @u";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", userID);

                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int mediaID = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            int releaseYear = reader.GetInt32(3);

                            int creatorID = reader.GetInt32(4);
                            string creatorUsername = reader.GetString(5);
                            string creatorHash = reader.GetString(6);

                            User creator = new User(creatorUsername, creatorID, creatorHash);
                            MediaEntry media = new MediaEntry(mediaID, title, desc, releaseYear, creator);

                            favourites.Add(media);
                        }
                    }
                }
            }

            return favourites;
        }
    }
}