using Moq;
using TaskManager.Application.UseCases;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using System.Threading.Tasks;

public class CreateTaskCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Add_TaskItem()
    {
        var mockRepo = new Mock<ITaskRepository>();
        var handler = new CreateTaskCommandHandler(mockRepo.Object);
        var command = new CreateTaskCommand("Test Task", "Test Description", DateTime.Now, 1);

        await handler.HandleAsync(command);

        mockRepo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
    }
}