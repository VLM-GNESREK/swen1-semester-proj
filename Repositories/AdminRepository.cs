// Repositories/AdminRepository.cs

using Npgsql;

namespace Treasure_Bay.Repositories
{
    public class AdminRepository
    {
        public void ResetDatabase()
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "TRUNCATE TABLE users, media, ratings RESTART IDENTITY CASCADE;";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DropDatabase()
        {
            using(var conn = new NpgsqlConnection(DataBaseSetup.ConnectionString))
            {
                conn.Open();
                var sql = "DROP TABLE IF EXISTS favourites, ratings, media, users CASCADE;";

                using(var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
