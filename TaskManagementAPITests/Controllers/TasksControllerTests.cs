using System;
using System.Collections.Generic;
using Xunit;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;
using TaskManagementAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagementAPI.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly TasksController _taskController;
        private readonly TaskRepository _taskRepository;

        public TasksControllerTests()
        {
            _taskRepository = new TaskRepository();
            _taskController = new TasksController(_taskRepository);
        }

        // Test case 1: Ensure task cannot be marked as complete if dependencies are not completed.
        [Fact]
        public void CannotMarkTaskComplete_IfDependenciesNotCompleted()
        {
            var task1 = _taskRepository.Add(new TaskItem { Title = "Task 1", DueDate = DateTime.Now.AddDays(1) });
            var task2 = _taskRepository.Add(new TaskItem { Title = "Task 2", DueDate = DateTime.Now.AddDays(2), Dependencies = new List<int> { task1.Id } });

            var result = _taskController.CompleteTask(task2.Id);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        // Test case 2: Detect circular dependencies between task.
        [Fact]
        public void DetectCircularDependency_WhenTaskDependsOnItself()
        {
            var task = _taskRepository.Add(new TaskItem
            {
                Title = "Task with Circular Dependency",
                DueDate = DateTime.Now.AddDays(1),
            });

            task.Dependencies = new List<int> { task.Id };

            var result = _taskController.CreateTask(task);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        // Test case 3: Ensure task cannot have a due date in the past.
        [Fact]
        public void CannotCreateTask_WithDueDateInThePast()
        {
            var task = new TaskItem
            {
                Title = "Task with Past Due Date",
                DueDate = DateTime.Now.AddDays(-1)
            };

            var result = _taskController.CreateTask(task);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
