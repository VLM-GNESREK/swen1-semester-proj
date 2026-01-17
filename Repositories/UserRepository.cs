// Repositories/UserRepository.cs

using System.Data;
using System.Data.SqlTypes;
using Npgsql;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public class UserRepository : IUserRepository
    {

        // ## METHODS ##

        public User? GetUserByUsername(string username) 
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "SELECT user_id, password_hash FROM users WHERE username = @u";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string dbHash = reader.GetString(1);

                            return new User(username, id, dbHash);
                        }
                    }
                }
            }
            return null;
        }

        public int CreateUser(string username, string hashedPassword)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "INSERT INTO users (username, password_hash) VALUES (@u, @p) RETURNING user_id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", hashedPassword);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public User? GetUserByID(int userID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "SELECT user_id, username, password_hash FROM users WHERE user_id = @id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", userID);

                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            return new User(
                                reader.GetString(1),
                                reader.GetInt32(0),
                                reader.GetString(2)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public void AddFavourite(int userID, int mediaID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "INSERT INTO favourites (user_id, media_id) VALUES (@u, @m) ON CONFLICT DO NOTHING";

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
                var sql =  "DELETE FROM favourites WHERE user_id = @u AND media_id = @m";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", userID);
                    cmd.Parameters.AddWithValue("m", mediaID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<MediaEntry> GetFavourites(int userID)
        {
            List<MediaEntry> favourites = new List<MediaEntry>();

            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"
                            SELECT
                                m.media_id, m.title, m.description, m.release_year, m.user_id,
                                u.username, u.password_hash
                            FROM favourites f
                            JOIN media m ON f.media_id = m.media_id
                            JOIN users u ON f.user_id = u.user_id
                            WHERE f.user_id = @id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", userID);

                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int mediaID = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            string desc = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            int release_year = reader.GetInt32(3);

                            int creatorID = reader.GetInt32(4);
                            string creatorName = reader.GetString(5);
                            string creatorHash = reader.GetString(6);
                            User creator = new User(creatorName, creatorID, creatorHash);

                            MediaEntry media = new MediaEntry(mediaID, title, desc, release_year, creator);
                            favourites.Add(media);
                        }
                    }
                }
            }
            return favourites;
        }
    }
}