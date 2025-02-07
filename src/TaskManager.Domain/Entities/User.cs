using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Constructor for creating a new user
        //public User(string username, string passwordHash)
        public User(string username, string passwordHash)
        {
            Id = new Random().Next();
            Username = username ?? throw new ArgumentNullException(nameof(username));
            //PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            PasswordHash = passwordHash;
        }

        // Factory method for creating a user
        public static User Create(string username, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentNullException(nameof(rawPassword), "Password cannot be empty.");

            return new User(username, rawPassword);
            //return new User(username, Password.Create(rawPassword).Hash);
            //return new User(username, Password.Create(rawPassword));
        }
    }
}
