using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.UseCases
{
    public interface ICreateTaskCommandHandler
    {
        TaskItem HandleAsync(CreateTaskCommand command);
    }

    public record CreateTaskCommand(string Title, string Description, DateTime DueDate, int UserId);

}
