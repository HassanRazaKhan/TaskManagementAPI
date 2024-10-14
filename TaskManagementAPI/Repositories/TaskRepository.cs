using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskRepository
    {
        private readonly ConcurrentDictionary<int, TaskItem> _tasks = new ConcurrentDictionary<int, TaskItem>();
        private int _currentId = 0;

        public IEnumerable<TaskItem> GetAll() => _tasks.Values;

        public TaskItem Get(int id) => _tasks.TryGetValue(id, out var task) ? task : null;

        public TaskItem Add(TaskItem task)
        {
            task.Id = _currentId++;
            _tasks.TryAdd(task.Id, task);
            return task;
        }

        public bool Update(int id, TaskItem updateTask) 
        { 
            if (_tasks.ContainsKey(id))
            {
                _tasks[id] = updateTask;
                return true;
            }
            return false;
        }

        public bool Delete(int id) => _tasks.TryRemove(id, out _);

        public bool AllDependenciesCompleted(TaskItem task) 
        {
            return task.Dependencies.All(depId => _tasks.ContainsKey(depId) && _tasks[depId].IsCompleted);
        }

        public bool HasCircularDependency(int taskId, List<int> dependencies)
        {
            return dependencies.Any(depId => depId == taskId || HasCircularDependency(depId, _tasks[depId]?.Dependencies ?? new List<int>()));
        }

    }
}
