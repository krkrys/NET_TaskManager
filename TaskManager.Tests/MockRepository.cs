using TaskManager.BusinessLogic;

namespace TaskManager.Tests
{
    public class MockRepository : IRepository
    {
        private int _taskId = 0;
        private List<TaskItem> _tasks = new List<TaskItem>();

        private List<User> _users = new List<User> { new User(1, "Ja") };

        public async Task<User[]> GetAllUsersAsync() => _users.ToArray();

        public async Task<User> GetUserByIdAsync(int userId) => _users.FirstOrDefault(u => u.Id == userId);

        public async Task<int> CreateTaskItemAsync(TaskItem newTaskItem)
        {
            var newTask = new TaskItem(newTaskItem.Id == 0 ? ++_taskId : newTaskItem.Id, newTaskItem.Description, newTaskItem.CreatedBy, newTaskItem.DueDate);
            _tasks.Add(newTask);
            return newTask.Id;
        }

        public async Task<bool> UpdateTaskItemAsync(TaskItem newTaskItem)
        {
            var result = await DeleteTaskItemAsync(newTaskItem.Id);
            if (result)
                _tasks.Add(newTaskItem);
            return result;
        }

        public async Task<bool> DeleteTaskItemAsync(int taskItemId)
        {
            var task = await GetTaskItemByIdAsync(taskItemId);
            return _tasks.Remove(task);
        }

        public async Task<TaskItem> GetTaskItemByIdAsync(int taskItemId) => _tasks.Find(t => t.Id == taskItemId);

        public async Task<TaskItem[]> GetAllTaskItemsAsync() => _tasks.ToArray();

        public async Task<TaskItem[]> GetTaskItemsByStatusAsync(TaskItemStatus status) => _tasks.Where(t => t.Status == status).ToArray();

        public async Task<TaskItem[]> GetTaskItemsByDescriptionAsync(string description) =>
            _tasks.FindAll(t => t.Description.Contains(description, StringComparison.InvariantCultureIgnoreCase)).ToArray();
    }
}