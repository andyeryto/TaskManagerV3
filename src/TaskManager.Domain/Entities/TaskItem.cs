using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public Enums.TaskItemStatus Status { get; set; }
        public string Priority { get; set; } // AI-Based Priority
        public string Sentiment { get; set; } // AI-Based Sentiment

        public TaskItem(string title, string description, int userId, DateTime dueDate, Enums.TaskItemStatus status, int id = 0)
        {
            Id = id != 0 ? new Random().Next() : id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            UserId = userId;
            DueDate = DateTime.UtcNow;
            Status = status;
        }

        public static TaskItem Create(string title, string description, int userId, DateTime dueDate, Enums.TaskItemStatus taskItemStatus, int? id = 0)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title), "Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description), "Description cannot be empty.");

            if (userId == 0)
                throw new ArgumentException("UserId cannot be 0.", nameof(userId));

            if (userId < 0)
                throw new ArgumentException("UserId cannot be negative.", nameof(userId));

            if (dueDate == DateTime.MinValue)
                throw new ArgumentException("DueDate cannot be DateTime.MinValue.", nameof(dueDate));

            if (taskItemStatus == Enums.TaskItemStatus.None)
                throw new ArgumentException("TaskItemStatus cannot be None.", nameof(taskItemStatus));

            return new TaskItem(title, description, userId, dueDate, taskItemStatus);
        }

        public void Update(string title, string description)
        {
            Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentNullException(nameof(title)) : title;
            Description = string.IsNullOrWhiteSpace(description) ? throw new ArgumentNullException(nameof(description)) : description;
            DueDate = DateTime.UtcNow;
        }
    }
}
