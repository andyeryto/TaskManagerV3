using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(TaskItem task)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(
                    "INSERT INTO Tasks (Title, Description, DueDate, UserId, Status) VALUES (@Title, @Description, @DueDate, @UserId, @Status)",
                    connection))
                {
                    command.Parameters.AddWithValue("@Title", task.Title);
                    command.Parameters.AddWithValue("@Description", task.Description);
                    command.Parameters.AddWithValue("@DueDate", task.DueDate);
                    command.Parameters.AddWithValue("@UserId", task.UserId);
                    // Convert Enum to int before inserting into the database
                    command.Parameters.AddWithValue("@Status", (int)task.Status);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var tasks = new List<TaskItem>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT Id, Title, Description, DueDate, UserId, Status FROM Tasks", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tasks.Add(TaskItem.Create(
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(4),
                            reader.GetDateTime(3),
                            (TaskItemStatus)reader.GetInt32(5)
                        ));
                    }
                }
            }
            return tasks;
        }

        public async Task<TaskItem> GetByIdAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT Id, Title, Description, DueDate, UserId, Status FROM Tasks WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return TaskItem.Create(
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(4),
                                reader.GetDateTime(3),
                                (TaskItemStatus)reader.GetInt32(5), reader.GetInt32(0)
                            );
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateAsync(TaskItem task)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(
                    "UPDATE Tasks SET Title = @Title, Description = @Description, DueDate = @DueDate, Status = @Status WHERE Id = @Id",
                    connection))
                {
                    command.Parameters.AddWithValue("@Id", task.Id);
                    command.Parameters.AddWithValue("@Title", task.Title);
                    command.Parameters.AddWithValue("@Description", task.Description);
                    command.Parameters.AddWithValue("@DueDate", task.DueDate);
                    command.Parameters.AddWithValue("@Status", task.Status);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("DELETE FROM Tasks WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
