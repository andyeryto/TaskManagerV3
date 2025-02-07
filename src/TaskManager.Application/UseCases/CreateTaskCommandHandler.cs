using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.UseCases
{
    public class CreateTaskCommandHandler : ICreateTaskCommandHandler
    {
        private readonly ITaskRepository _taskRepository;

        public CreateTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task HandleAsync(CreateTaskCommand command)
        {
            var taskItem = TaskItem.Create(
                command.Title,
                command.Description,
                command.UserId,
                command.DueDate,
                TaskManager.Domain.Enums.TaskItemStatus.Pending
            );

            await _taskRepository.AddAsync(taskItem);
        }

        TaskItem ICreateTaskCommandHandler.HandleAsync(CreateTaskCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
