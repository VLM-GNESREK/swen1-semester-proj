// Services/MediaService.cs

using System.Data.SqlTypes;
using Npgsql;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Services
{
    public class MediaService
    {
        public MediaEntry CreateMedia(string title, string description, int releaseYear, User user)
        {
            int newID = 0;

            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "INSERT INTO media (title, description, release_year, user_id) VALUES (@u, @p, @r, @z) RETURNING media_id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", title);
                    cmd.Parameters.AddWithValue("p", description);
                    cmd.Parameters.AddWithValue("r", releaseYear);
                    cmd.Parameters.AddWithValue("z", user.UserID);

                    newID = Convert.ToInt32(cmd.ExecuteScalar());
                }

            }

            MediaEntry newMedia = new MediaEntry(newID, title, description, releaseYear, user);
            return newMedia;
        }

        public MediaEntry? FindMedia(int mediaID)
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
                    cmd.Parameters.AddWithValue("m", mediaID);

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

                            MediaEntry foundMedia = new MediaEntry(mediaID, title, desc, releaseYear, user);
                            returnMedia = foundMedia;
                        }
                    }
                }
            }
            return returnMedia;
        }

        public void DeleteMedia(int mediaID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "DELETE FROM media WHERE media_id = @m";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m", mediaID);
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
                var sql = @"
                    SELECT 
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
    }
}