namespace TaskManager.BusinessLogic
{
    public class TaskManagerService
    {
        private static int _id=0;

        private List<TaskItem> _tasks = new List<TaskItem>();

        public async Task<TaskItem> AddAsync(string description, DateTime? dueDate)
        {
            var task = new TaskItem(description, dueDate);
            _tasks.Add(task);
            return task;
        }

        public async Task<bool> RemoveAsync(int taskId)
        {
            var task = await GetAsync(taskId);
            if (task != null)
                return _tasks.Remove(task);
            return false;
        }

        public async Task<TaskItem> GetAsync(int taskId)
        {
            return _tasks.Find(t => t.Id == taskId);
        }

        public async Task<TaskItem[]> GetAllAsync()
        {
            return _tasks.ToArray();
        }

        public async Task<TaskItem[]> GetAllAsync(TaskItemStatus status)
        {
            return _tasks.FindAll(t => t.Status == status).ToArray();
        }

        public async Task<TaskItem[]> GetAllAsync(string description)
        {
            // Przeciążona wersja Contains przyjmuje drugi parametr jako opcje porównania tekstu,
            // gdzie możemy wskazać, aby przy porównaniu pomijać wielkość liter
            return _tasks.FindAll(t => t.Description.Contains(description, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        public async Task<bool> ChangeStatusAsync(int taskId, TaskItemStatus newStatus)
        {
            var task = await GetAsync(taskId);
            if (task == null || task?.Status == newStatus)
                return false;

            switch (newStatus)
            {
                case TaskItemStatus.ToDo:
                    return task.Open();
                case TaskItemStatus.InProgress:
                    return task.Start();
                case TaskItemStatus.Done:
                    return task.Done();
                default:
                    return false;
            }
            
        }
    }
}