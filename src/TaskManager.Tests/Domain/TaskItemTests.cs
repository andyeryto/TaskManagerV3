using Xunit;
using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Application.Services;
using System;

namespace TaskManager.Tests.Domain
{
    public class TaskItemTests
    {

        [Fact]
        public void Create_ShouldThrowException_WhenTitleIsNull()
        {
            Action act = () => TaskItem.Create(null!, "Description", new Random().Next(), DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
            act.Should().Throw<ArgumentNullException>().WithMessage("*title*");
        }

        [Fact]
        public void Create_ShouldThrowException_WhenDescriptionIsNull()
        {
            Action act = () => TaskItem.Create("Title", null!, new Random().Next(), DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
            act.Should().Throw<ArgumentNullException>().WithMessage("*description*");
        }

        [Fact]
        public void Create_ShouldAssignUniqueId()
        {
            var task = TaskItem.Create("Task Title", "Task Description", new Random().Next(), DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
            task.Id.Should().BePositive();
        }

        [Fact]
        public void Create_ShouldAssignCorrectUserId()
        {
            var userId = new Random().Next();
            var task = TaskItem.Create("Task Title", "Task Description", userId, DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
            task.UserId.Should().Be(userId);
        }

        [Fact]
        public void Update_ShouldModifyTitle_And_UpdateTimestamp()
        {
            var task = TaskItem.Create("Old Title", "Old Description", new Random().Next(), DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
            var dueDate = task.DueDate;

            task.Update("New Title", "New Description");

            task.Title.Should().Be("New Title");
            task.Description.Should().Be("New Description");
            task.DueDate.Should().BeAfter(dueDate);
        }
    }
}
