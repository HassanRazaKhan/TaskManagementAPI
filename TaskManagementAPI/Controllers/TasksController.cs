using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase 
    {
        private readonly TaskRepository _taskRepository;
        public TasksController(TaskRepository taskepository)
        {
            _taskRepository = new TaskRepository();
        }

        [HttpGet]
        public IEnumerable<TaskItem> GetTasks() 
        {
            return _taskRepository.GetAll();
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id) 
        {
            var task = _taskRepository.Get(id);
            if(task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskItem task) 
        {
            if(string.IsNullOrWhiteSpace(task.Title) || task.Title.Length > 100)
            {
                return BadRequest("Title must be provided and should not exceed 100 characters.");
            }
            if(task.DueDate < DateTime.Now)
            {
                return BadRequest("DueDate should not be in the past.");
            }
            if (task.Dependencies.Contains(task.Id))
            {
                return BadRequest("The task cannot depend on itself.");
            }
            if (_taskRepository.HasCircularDependency(task.Id, task.Dependencies))
            {
                return BadRequest("Circular Dependency Detected.");
            }
            var createdTask = _taskRepository.Add(task);
            return CreatedAtAction(nameof(GetTask), new {id = createdTask.Id}, createdTask);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem updatedTask) 
        {
            if (updatedTask.DueDate < DateTime.Now)
            {
                return BadRequest("DueDate should not be in the past.");
            }
            if (_taskRepository.HasCircularDependency(id, updatedTask.Dependencies))
            {
                return BadRequest("Circular Dependency Detected.");
            }
            if(!_taskRepository.Update(id, updatedTask))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            if(!_taskRepository.Delete(id))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/complete")]
        public IActionResult CompleteTask(int id)
        {
            var task = _taskRepository.Get(id);
            if (task == null)
            {
                return NotFound();
            }

            if (task.Dependencies.Any(depId => !_taskRepository.Get(depId)?.IsCompleted ?? false))
            {
                return BadRequest("Cannot mark task as completed until all dependencies are completed.");
            }

            task.IsCompleted = true;
            return Ok(task);
        }


    }
}
