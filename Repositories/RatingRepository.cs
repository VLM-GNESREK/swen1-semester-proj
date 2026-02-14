// Repositories/RatingRepository.cs

using Npgsql;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        public int CreateRating(User reviewer, MediaEntry media, int stars, string comment)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"
                            INSERT INTO ratings 
                                (user_id, media_id, star_value, comment)
                            VALUES (@u, @m, @s, @c)
                            RETURNING rating_id";
                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", reviewer.UserID);
                    cmd.Parameters.AddWithValue("m", media.MediaID);
                    cmd.Parameters.AddWithValue("s", stars);
                    cmd.Parameters.AddWithValue("c", comment);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<Rating> GetRatingsByMediaID(MediaEntry media)
        {
            List<Rating> ratings = new List<Rating>();
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"
                            SELECT
                                r.rating_id, r.star_value, r.comment, r.user_id, r.media_id,
                                u.username, password_hash
                            FROM ratings r
                            JOIN users u ON r.user_id = u.user_id
                            WHERE r.media_id = @m";
                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m", media.MediaID);

                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int ratingID = reader.GetInt32(0);
                            int stars = reader.GetInt32(1);
                            string comment = reader.GetString(2);
                            int userID = reader.GetInt32(3);
                            int mediaID = reader.GetInt32(4);
                            string username = reader.GetString(5);
                            string pw_hash = reader.GetString(6);

                            User user = new User(username, userID, pw_hash);
                            Rating rating = new Rating(ratingID, user, media, stars, comment);
                            ratings.Add(rating);
                        }
                    }
                }
            }
            return ratings;
        }

        public List<Rating> GetRatingsByUserID(User reviewer)
        {
            List<Rating> ratings = new List<Rating>();
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"
                            SELECT
                                r.rating_id, r.star_value, r.comment, r.media_id,
                                m.title, m.description, m.release_year, m.user_id,
                                u.username, u.password_hash
                            FROM ratings r
                            JOIN media m ON r.media_id = m.media_id
                            JOIN users u ON m.user_id = u.user_id
                            WHERE r.user_id = @u";
                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", reviewer.UserID);
                    using(var reader = cmd.ExecuteReader())
                    {
                        int ratingID = reader.GetInt32(0);
                        int stars = reader.GetInt32(1);
                        string comment = reader.GetString(2);
                        int mediaID = reader.GetInt32(3);
                        string title = reader.GetString(4);
                        string desc = reader.GetString(5);
                        int r_year = reader.GetInt32(6);
                        int userID = reader.GetInt32(7);
                        string username = reader.GetString(8);
                        string pw_hash = reader.GetString(9);

                        User user = new User(username, userID, pw_hash);
                        MediaEntry media = new MediaEntry(mediaID, title, desc, r_year, user);
                        Rating rating = new Rating(ratingID, reviewer, media, stars, comment);

                        ratings.Add(rating);
                    }
                }
            }
            return ratings;
        }

        public void UpdateRating(Rating rating)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "UPDATE ratings SET star_value = @s, comment = @c WHERE rating_id = @id";
                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("s", rating.StarValue);
                    cmd.Parameters.AddWithValue("c", rating.Comment);
                    cmd.Parameters.AddWithValue("id", rating.RatingID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRating(Rating rating)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "DELETE FROM ratings WHERE rating_id = @id";
                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", rating.RatingID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Rating? GetRatingByID(int ratingID)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = @"
                            SELECT
                                r.rating_id, r.star_value, r.comment, r.user_id, r.media_id,
                                m.title, m.description, m.release_year, m.user_id,
                                u.username, u.password_hash
                            FROM ratings r
                            JOIN media m ON r.media_id = m.media_id
                            JOIN users u ON r.user_id = u.user_id
                            WHERE r.rating_id = @id";
                
                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", ratingID);
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            // 0: rating_id is passed in
                            int stars = reader.GetInt32(1);
                            string comment = reader.GetString(2);
                            int reviewerID = reader.GetInt32(3);
                            int mediaID = reader.GetInt32(4);
                            
                            string title = reader.GetString(5);
                            string desc = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            int r_year = reader.GetInt32(7);
                            int creatorID = reader.GetInt32(8);
                            
                            string username = reader.GetString(9);
                            string pw_hash = reader.GetString(10);
                            User reviewer = new User(username, reviewerID, pw_hash);
                            User creatorPlaceholder = new User("Unknown", creatorID, "");
                            MediaEntry media = new MediaEntry(mediaID, title, desc, r_year, creatorPlaceholder);

                            return new Rating(ratingID, reviewer, media, stars, comment);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}