// DataBaseSetup.cs

using System.Data.SqlTypes;
using Npgsql;

public class DataBaseSetup
{
    public const string ConnectionString = "Host=localhost; Port=5432; Username=postgres; Password=mysecretpassword; Database=postgres";

    public static void InitialiseDatabase()
    {
        using(var conn = new NpgsqlConnection(ConnectionString))
        {
            Console.WriteLine("Opening database connection...");
            conn.Open();

            var sql = @"
                    CREATE TABLE IF NOT EXISTS users 
                    (
                        user_id SERIAL PRIMARY KEY,
                        username VARCHAR(255) UNIQUE NOT NULL,
                        password_hash TEXT NOT NULL
                    );
            ";

            using(var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }

            sql = @"
                CREATE TABLE IF NOT EXISTS media
                (
                    media_id SERIAL PRIMARY KEY,
                    title TEXT NOT NULL,
                    description TEXT,
                    release_year INTEGER,
                    media_type TEXT,
                    genre TEXT, 
                    age_restriction INTEGER,
                    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE
                );
            ";

            using(var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }

            sql = @"
                CREATE TABLE IF NOT EXISTS ratings
                (
                    rating_id SERIAL PRIMARY KEY,
                    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
                    media_id INTEGER NOT NULL REFERENCES media(media_id) ON DELETE CASCADE,
                    star_value INTEGER NOT NULL,
                    comment TEXT,
                    UNIQUE(user_id, media_id)
                );
            ";

            using(var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }

            sql = @"
                CREATE TABLE IF NOT EXISTS favourites
                (
                    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
                    media_id INTEGER NOT NULL REFERENCES media(media_id) ON DELETE CASCADE,
                    PRIMARY KEY (user_id, media_id)
                );
            ";

            using(var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Database tables have been set up successfully!");
        }
    }
}