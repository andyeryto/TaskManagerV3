using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.UseCases
{
    public interface IUpdateTaskCommandHandler
    {
        TaskItem HandleAsync(UpdateTaskCommand command);
    }

    public record UpdateTaskCommand(int TaskId, string Title, string Description, DateTime DueDate);

}
