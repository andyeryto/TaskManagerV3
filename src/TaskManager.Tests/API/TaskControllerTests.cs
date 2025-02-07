using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Controllers;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using Xunit;

public class TaskControllerTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _controller = new TaskController(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task GetTasks_ReturnsListOfTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem("Task 1", "Description 1", 1, DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending),
            new TaskItem("Task 2", "Description 2", 1, DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending)
        };
        _mockTaskRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTasks();

        // Assert
        var taskList = Assert.IsType<List<TaskItem>>(result);
        Assert.Equal(2, taskList.Count);
    }

    [Fact]
    public async Task GetTask_ReturnsTaskItem_WhenFound()
    {
        // Arrange
        var task = new TaskItem("Test Task", "Description", 1, DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _controller.GetTask(1);
        var okResult = Assert.IsType<ActionResult<TaskItem>>(result);
        var returnedTask = Assert.IsType<TaskItem>(okResult.Value);

        // Assert
        Assert.Equal(1, returnedTask.Id);
    }

    [Fact]
    public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);

        // Act
        var result = await _controller.GetTask(99);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedAtAction()
    {
        // Arrange
        var task = new TaskItem("New Task", "Description", 1, DateTime.Now, TaskManager.Domain.Enums.TaskItemStatus.Pending);
        _mockTaskRepository.Setup(repo => repo.AddAsync(task)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateTask(task);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetTask", createdAtActionResult.ActionName);
    }
}
