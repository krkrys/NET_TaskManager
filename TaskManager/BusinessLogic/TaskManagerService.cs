namespace TaskManager.BusinessLogic
{
    public class TaskManagerService
    {
        private List<Task> _tasks = new List<Task>();

        public Task Add(string description, DateTime? dueDate)
        {
            var task = new Task(description, dueDate);
            _tasks.Add(task);
            return task;
        }

        public bool Remove(int taskId)
        {
            var task = Get(taskId);
            if (task != null)
                return _tasks.Remove(task);
            return false;
        }

        public Task Get(int taskId)
        {
            return _tasks.Find(t => t.Id == taskId);
        }

        public Task[] GetAll()
        {
            return _tasks.ToArray();
        }

        public Task[] GetAll(TaskStatus status)
        {
            return _tasks.FindAll(t => t.Status == status).ToArray();
        }

        public Task[] GetAll(string description)
        {
            // Przeciążona wersja Contains przyjmuje drugi parametr jako opcje porównania tekstu,
            // gdzie możemy wskazać, aby przy porównaniu pomijać wielkość liter
            return _tasks.FindAll(t => t.Description.Contains(description, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        public bool ChangeStatus(int taskId, TaskStatus newStatus)
        {
            var task = Get(taskId);
            if (task == null || task?.Status == newStatus)
                return false;

            switch (newStatus)
            {
                case TaskStatus.ToDo:
                    return task.Open();
                case TaskStatus.InProgress:
                    return task.Start();
                case TaskStatus.Done:
                    return task.Done();
                default:
                    return false;
            }
            
        }
    }
}