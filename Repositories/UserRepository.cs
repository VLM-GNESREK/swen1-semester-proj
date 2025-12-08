// Repositories/UserRepository.cs

using Npgsql;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Repositories
{
    public class UserRepository : IUserRepository
    {

        // ## METHODS ##

        public User? GetUserByUseranme(string username) 
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
                var sql = "INSERT INTO users (username, password_hash) VALUES (@u, @p) RETURNUNG user_id";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", hashedPassword);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}