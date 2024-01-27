namespace TaskManager.BusinessLogic
{
    public interface IRepository
    {
        Task<User[]> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<int> CreateTaskItemAsync(TaskItem newTaskItem);
        Task<bool> UpdateTaskItemAsync(TaskItem newTaskItem);
        Task<bool> DeleteTaskItemAsync(int taskItemId);
        Task<TaskItem?> GetTaskItemByIdAsync(int taskItemId);
        Task<TaskItem[]> GetAllTaskItemsAsync();
        Task<TaskItem[]> GetTaskItemsByStatusAsync(TaskItemStatus status);
        Task<TaskItem[]> GetTaskItemsByDescriptionAsync(string description);
    }
}