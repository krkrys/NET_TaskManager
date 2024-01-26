namespace TaskManager.BusinessLogic
{
    public class TaskManagerService
    {
        private readonly IRepository _repository;

        public TaskManagerService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<TaskItem> AddAsync(string description, int createdBy, DateTime? dueDate)
        {
            var user = await _repository.GetUserByIdAsync(createdBy);
            var task = new TaskItem(0, description, user, dueDate);
            var id = await _repository.CreateTaskItemAsync(task);
            return await GetAsync(id);
        }

        public async Task<bool> RemoveAsync(int taskId)
        {
            var task = await GetAsync(taskId);
            if (task != null)
                return await _repository.DeleteTaskItemAsync(task.Id);
            return false;
        }

        public async Task<TaskItem?> GetAsync(int taskId)
        {
            return await _repository.GetTaskItemByIdAsync(taskId);
        }

        public async Task<TaskItem[]> GetAllAsync()
        {
            return await _repository.GetAllTaskItemsAsync();
        }

        public async Task<TaskItem[]> GetAllAsync(TaskItemStatus itemStatus)
        {
            return await _repository.GetTaskItemsByStatusAsync(itemStatus);
        }

        public async Task<TaskItem[]> GetAllAsync(string description)
        {
            return await _repository.GetTaskItemsByDescriptionAsync(description);
        }

        public async Task<bool> ChangeStatusAsync(int taskId, TaskItemStatus newStatus)
        {
            var task = await GetAsync(taskId);
            if (task == null || task?.Status == newStatus)
                return false;

            var result = ChangeStatus(task, newStatus);
            if (result)
            {
                return await _repository.UpdateTaskItemAsync(task);
            }

            return false;
        }

        private bool ChangeStatus(TaskItem task, TaskItemStatus newStatus)
        {
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

        public async Task<User[]> GetAllUsersAsync() => await _repository.GetAllUsersAsync();

        public async Task<bool> AssignToAsync(int taskId, int? userId)
        {
            var task = await GetAsync(taskId);
            if (task == null)
                return false;

            User? user = null;
            if (userId.HasValue)
            {
                user = await _repository.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return false;
            }
            task.AssignTo(user);
            return await _repository.UpdateTaskItemAsync(task);
        }
    }
}