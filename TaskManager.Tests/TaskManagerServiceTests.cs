using TaskManager.BusinessLogic;
//TaskStatus już istnieje w przestrzeni System.Threading.Tasks, która jest automatycznie importowana.
//Musimy rozwiązać konflikt nazw stosując alias.
using TaskItemStatus = TaskManager.BusinessLogic.TaskItemStatus;

namespace TaskManager.Tests
{
    public class TaskManagerServiceTests
    {
        [Fact]
        public async Task Should_AddTask_ToTaskListAsync()
        {
            var service = new TaskManagerService();
            
            var task = await service.AddAsync("Test task", DateTime.Now.AddDays(5));
            
            Assert.NotNull(task);
            Assert.Single(await service.GetAllAsync());
        }

        [Fact]
        public async Task Should_RemoveTask_ByTaskIdAsync()
        {
            var service = new TaskManagerService();
            var task = await service.AddAsync("Test task", null);

            bool result = await service.RemoveAsync(task.Id);

            Assert.True(result);
            Assert.Empty(await service.GetAllAsync());
        }

        [Fact]
        public async Task Should_NotRemoveTask_WhenTaskIdDoesNotExistAsync()
        {
            var service = new TaskManagerService();
            await service.AddAsync("Test task", null);

            bool result = await service.RemoveAsync(999);

            Assert.False(result);
            Assert.Single(await service.GetAllAsync());
        }

        [Fact]
        public async Task Should_GetTask_ByTaskIdAsync()
        {
            var service = new TaskManagerService();
            var task = await service.AddAsync("Test task", null);

            var retrievedTask = await service.GetAsync(task.Id);

            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Id, retrievedTask.Id);
        }

        [Fact]
        public async Task Should_GetAllTasks_WithNoFilterAsync()
        {
            var service = new TaskManagerService();
            await service.AddAsync("Test task 1", null);
            await service.AddAsync("Test task 2", null);

            var tasks = await service.GetAllAsync();

            Assert.Equal(2, tasks.Length);
        }

        [Fact]
        public async Task Should_GetTasks_ByStatusAsync()
        {
            var service = new TaskManagerService();
            var task1 = await service.AddAsync("Test task 1", null);
            task1.Start();
            await service.AddAsync("Test task 2", null);

            var inProgressTasks = await service.GetAllAsync(TaskItemStatus.InProgress);

            Assert.Single(inProgressTasks);
            Assert.Equal(task1.Id, inProgressTasks.First().Id);
        }

        [Fact]
        public async Task Should_GetTasks_ByDescriptionAsync()
        {
            var service = new TaskManagerService();
            await service.AddAsync("Unique test task", null);
            await service.AddAsync("Test task 2", null);

            var tasks = await service.GetAllAsync("Unique");

            Assert.Single(tasks);
            Assert.Equal("Unique test task", tasks.First().Description);
        }

        [Fact]
        public async Task Should_ChangeTaskStatus_WhenValidAsync()
        {
            var service = new TaskManagerService();
            var task = await service.AddAsync("Test task", null);

            bool result = await service.ChangeStatusAsync(task.Id, TaskItemStatus.InProgress);

            Assert.True(result);
            Assert.Equal(TaskItemStatus.InProgress, task.Status);
        }

        [Fact]
        public async Task Should_NotChangeTaskStatus_WhenInvalidTransitionAsync()
        {
            var service = new TaskManagerService();
            var task = await service.AddAsync("Test task", null);

            bool result = await service.ChangeStatusAsync(task.Id, TaskItemStatus.Done);

            Assert.False(result);
            Assert.Equal(TaskItemStatus.ToDo, task.Status);
        }

        [Fact]
        public async Task Should_NotChangeTaskStatus_WhenTaskIdDoesNotExistAsync()
        {
            var service = new TaskManagerService();
            await service.AddAsync("Test task", null);

            bool result = await service.ChangeStatusAsync(999, TaskItemStatus.Done);

            Assert.False(result);
        }
    }
}
