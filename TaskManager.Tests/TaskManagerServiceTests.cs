using TaskManager.BusinessLogic;

namespace TaskManager.Tests
{
    public class TaskManagerServiceTests
    {
        private readonly int _createdBy = 1;

        [Fact]
        public async Task Should_AddTask_ToTaskList()
        {
            var service = new TaskManagerService(new MockRepository());
            
            var task = await service.AddAsync("Test task", _createdBy, DateTime.Now.AddDays(5));
            
            Assert.NotNull(task);
            Assert.Single(await service.GetAllAsync());
        }

        [Fact]
        public async Task Should_RemoveTask_ByTaskId()
        {
            var service = new TaskManagerService(new MockRepository());
            var task = await service.AddAsync("Test task", _createdBy, null);

            bool result = await service.RemoveAsync(task.Id);

            Assert.True(result);
            Assert.Empty(await service.GetAllAsync());
        }

        [Fact]
        public async Task Should_NotRemoveTask_WhenTaskIdDoesNotExist()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Test task", _createdBy, null);

            bool result = await service.RemoveAsync(999);

            Assert.False(result);
            Assert.Single(await service.GetAllAsync());
        }

        [Fact]
        public async Task Should_GetTask_ByTaskId()
        {
            var service = new TaskManagerService(new MockRepository());
            var task = await service.AddAsync("Test task", _createdBy, null);

            var retrievedTask = await service.GetAsync(task.Id);

            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Id, retrievedTask.Id);
        }

        [Fact]
        public async Task Should_GetAllTasks_WithNoFilter()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Test task 1", _createdBy, null);
            await service.AddAsync("Test task 2", _createdBy, null);

            var tasks = await service.GetAllAsync();

            Assert.Equal(2, tasks.Length);
        }

        [Fact]
        public async Task Should_GetTasks_ByStatus()
        {
            var service = new TaskManagerService(new MockRepository());
            var task1 = await service.AddAsync("Test task 1", _createdBy, null);
            task1.Start();
            await service.AddAsync("Test task 2", _createdBy, null);

            var inProgressTasks = await service.GetAllAsync(TaskItemStatus.InProgress);

            Assert.Single(inProgressTasks);
            Assert.Equal(task1.Id, inProgressTasks.First().Id);
        }

        [Fact]
        public async Task Should_GetTasks_ByDescription()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Unique test task", _createdBy, null);
            await service.AddAsync("Test task 2", _createdBy, null);

            var tasks = await service.GetAllAsync("Unique");

            Assert.Single(tasks);
            Assert.Equal("Unique test task", tasks.First().Description);
        }

        [Fact]
        public async Task Should_ChangeTaskStatus_WhenValid()
        {
            var service = new TaskManagerService(new MockRepository());
            var task = await service.AddAsync("Test task", _createdBy, null);

            bool result = await service.ChangeStatusAsync(task.Id, TaskItemStatus.InProgress);

            Assert.True(result);
            Assert.Equal(TaskItemStatus.InProgress, task.Status);
        }

        [Fact]
        public async Task Should_NotChangeTaskStatus_WhenInvalidTransition()
        {
            var service = new TaskManagerService(new MockRepository());
            var task = await service.AddAsync("Test task", _createdBy, null);

            bool result = await service.ChangeStatusAsync(task.Id, TaskItemStatus.Done);

            Assert.False(result);
            Assert.Equal(TaskItemStatus.ToDo, task.Status);
        }

        [Fact]
        public async Task Should_NotChangeTaskStatus_WhenTaskIdDoesNotExist()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Test task", _createdBy, null);

            bool result = await service.ChangeStatusAsync(999, TaskItemStatus.Done);

            Assert.False(result);
        }

        [Fact]
        public async void Should_Assign_ExistingUser_To_ExistingTask()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Test task", _createdBy, null);

            var result = await service.AssignToAsync(1, _createdBy);

            var task = await service.GetAsync(1);
            Assert.True(result);
            Assert.Equal(_createdBy, task.AssignedTo.Id);
        }

        [Fact]
        public async void Should_Unassign_User_From_Task()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Test task", _createdBy, null);
            await service.AssignToAsync(1, _createdBy);

            var result = await service.AssignToAsync(1, null);

            var task = await service.GetAsync(1);
            Assert.True(result);
            Assert.Null(task.AssignedTo);
        }

        [Fact]
        public async void Should_NotAssign_NotExistingUser_To_ExistingTask()
        {
            var service = new TaskManagerService(new MockRepository());
            await service.AddAsync("Test task", _createdBy, null);

            var result = await service.AssignToAsync(1, 999);

            var task = await service.GetAsync(1);
            Assert.False(result);
            Assert.Null(task.AssignedTo);
        }

        [Fact]
        public async void Should_NotAssign_ExistingUser_To_NotExistingTask()
        {
            var service = new TaskManagerService(new MockRepository());

            var result = await service.AssignToAsync(1, 1);

            var task = await service.GetAsync(1);
            Assert.False(result);
            Assert.Null(task);
        }
    }
}
