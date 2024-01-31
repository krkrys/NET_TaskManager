using TaskManager.BusinessLogic;
//TaskStatus już istnieje w przestrzeni System.Threading.Tasks, która jest automatycznie importowana.
//Musimy rozwiązać konflikt nazw stosując alias.
using TaskStatus = TaskManager.BusinessLogic.TaskStatus;

namespace TaskManager.Tests
{
    public class TaskManagerServiceTests
    {
        [Fact]
        public void Should_AddTask_ToTaskList()
        {
            var service = new TaskManagerService();
            
            var task = service.Add("Test task", DateTime.Now.AddDays(5));
            
            Assert.NotNull(task);
            Assert.Single(service.GetAll());
        }

        [Fact]
        public void Should_RemoveTask_ByTaskId()
        {
            var service = new TaskManagerService();
            var task = service.Add("Test task", null);

            bool result = service.Remove(task.Id);

            Assert.True(result);
            Assert.Empty(service.GetAll());
        }

        [Fact]
        public void Should_NotRemoveTask_WhenTaskIdDoesNotExist()
        {
            var service = new TaskManagerService();
            service.Add("Test task", null);

            bool result = service.Remove(999);

            Assert.False(result);
            Assert.Single(service.GetAll());
        }

        [Fact]
        public void Should_GetTask_ByTaskId()
        {
            var service = new TaskManagerService();
            var task = service.Add("Test task", null);

            var retrievedTask = service.Get(task.Id);

            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Id, retrievedTask.Id);
        }

        [Fact]
        public void Should_GetAllTasks_WithNoFilter()
        {
            var service = new TaskManagerService();
            service.Add("Test task 1", null);
            service.Add("Test task 2", null);

            var tasks = service.GetAll();

            Assert.Equal(2, tasks.Length);
        }

        [Fact]
        public void Should_GetTasks_ByStatus()
        {
            var service = new TaskManagerService();
            var task1 = service.Add("Test task 1", null);
            task1.Start();
            service.Add("Test task 2", null);

            var inProgressTasks = service.GetAll(TaskStatus.InProgress);

            Assert.Single(inProgressTasks);
            Assert.Equal(task1.Id, inProgressTasks.First().Id);
        }

        [Fact]
        public void Should_GetTasks_ByDescription()
        {
            var service = new TaskManagerService();
            service.Add("Unique test task", null);
            service.Add("Test task 2", null);

            var tasks = service.GetAll("Unique");

            Assert.Single(tasks);
            Assert.Equal("Unique test task", tasks.First().Description);
        }

        [Fact]
        public void Should_ChangeTaskStatus_WhenValid()
        {
            var service = new TaskManagerService();
            var task = service.Add("Test task", null);

            bool result = service.ChangeStatus(task.Id, TaskStatus.InProgress);

            Assert.True(result);
            Assert.Equal(TaskStatus.InProgress, task.Status);
        }

        [Fact]
        public void Should_NotChangeTaskStatus_WhenInvalidTransition()
        {
            var service = new TaskManagerService();
            var task = service.Add("Test task", null);

            bool result = service.ChangeStatus(task.Id, TaskStatus.Done);

            Assert.False(result);
            Assert.Equal(TaskStatus.ToDo, task.Status);
        }

        [Fact]
        public void Should_NotChangeTaskStatus_WhenTaskIdDoesNotExist()
        {
            var service = new TaskManagerService();
            service.Add("Test task", null);

            bool result = service.ChangeStatus(999, TaskStatus.Done);

            Assert.False(result);
        }
    }
}
