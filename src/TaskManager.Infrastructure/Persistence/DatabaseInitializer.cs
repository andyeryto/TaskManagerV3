using System;
using System.Threading.Tasks;
using Npgsql;

namespace TaskManager.Infrastructure.Persistence
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(ConnectionFactory connectionFactory)
        {
            using (var connection = connectionFactory.CreateConnection() as NpgsqlConnection)
            {
                await connection.OpenAsync();

                var commandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id SERIAL PRIMARY KEY,
                    Username VARCHAR(100) NOT NULL UNIQUE,
                    Email VARCHAR(255) NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Tasks (
                    Id SERIAL PRIMARY KEY,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    DueDate TIMESTAMP NOT NULL,
                    UserId INT NOT NULL,
                    Status INT NOT NULL,
                    Priority VARCHAR(20),
                    Sentiment VARCHAR(20),
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                ";

                using (var command = new NpgsqlCommand(commandText, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
