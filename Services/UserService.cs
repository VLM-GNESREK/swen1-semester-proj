// Services/UserService.cs

using System.Linq.Expressions;
using Npgsql;
using Treasure_Bay.Classes;
using Treasure_Bay.DTO;

namespace Treasure_Bay.Services
{
    public class UserService
    {

        // ## METHODS ##

        public bool QueryUserExists(string _username)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "SELECT COUNT(*) FROM users WHERE username = @u";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", _username);
                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public User? Login(string _username, string _password)
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "SELECT user_id, password_hash FROM users WHERE username = @u";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", _username);

                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string dbHash = reader.GetString(1);

                            if(BCrypt.Net.BCrypt.Verify(_password, dbHash))
                            {
                                return new User(_username, id, dbHash);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public UserResponseDTO Register(string _username, string Password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
            int newID = 0;

            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "INSERT INTO users (username, password_hash) VALUES (@u, @p) RETURNING user_id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", _username);
                    cmd.Parameters.AddWithValue("p", hashedPassword);
                    newID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            User newUser = new User(_username, newID, hashedPassword);
            return new UserResponseDTO(newUser);
        }
    }
}