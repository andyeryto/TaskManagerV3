using Xunit;
using TaskManager.Application.Services;

public class TaskAIServiceTests
{
    [Fact]
    public void AnalyzeTaskPriority_Should_Return_HighPriority_When_Contains_Urgent()
    {
        var aiService = new TaskAIService();
        string result = aiService.AnalyzeTaskPriority("urgent task");
        Assert.Equal("High Priority", result);
    }
}