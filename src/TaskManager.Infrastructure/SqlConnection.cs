//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TaskManager.Infrastructure
//{
//    public class SqlConnection
//    {
//        private readonly string _connectionString;

//        public SqlConnection(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public async Task OpenConnectionAsync()
//        {
//            using (SqlConnection connection = new SqlConnection(_connectionString))
//            {
//                await connection.OpenAsync();
//                Console.WriteLine("Connection opened successfully.");

//                // Example query execution
//                using (SqlCommand command = new SqlCommand("SELECT GETDATE()", connection))
//                {
//                    object result = await command.ExecuteScalarAsync();
//                    Console.WriteLine($"Current Date from SQL Server: {result}");
//                }
//            }
//        }
//    }
//}
