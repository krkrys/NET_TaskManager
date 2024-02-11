//Task i TaskStatus już istnieją w przestrzeni System.Threading.Tasks, która jest automatycznie importowana.
//Musimy rozwiązać konflikt nazw stosując aliasy.
using Task = TaskManager.BusinessLogic.TaskItem;
using TaskItemStatus = TaskManager.BusinessLogic.TaskItemStatus;

namespace TaskManager.Tests
{
    public class TaskItemTests
    {
        [Fact]
        public void Should_CreateTask_WithAutoIncrementedId()
        {
            var task1 = new Task("Test task 1", null);
            var task2 = new Task("Test task 2", null);

            Assert.True(task1.Id > 0);
            Assert.Equal(task1.Id + 1, task2.Id);
        }

        [Fact]
        public void Should_SetCreationDate_WhenCreatingTask()
        {
            var task = new Task("Test task", null);
            var difference = DateTime.Now - task.CreationDate;

            Assert.True(difference.TotalSeconds < 1);
        }

        [Fact]
        public void Should_SetDueDate_WhenProvided()
        {
            var dueDate = DateTime.Now.AddDays(7);
            var task = new Task("Test task", dueDate);

            Assert.Equal(dueDate, task.DueDate);
        }

        [Fact]
        public void Should_SetStatusToTodo_WhenTaskIsCreated()
        {
            var task = new Task("Test task", null);

            Assert.Equal(TaskItemStatus.ToDo, task.Status);
        }

        [Fact]
        public void Should_ChangeStatus_ToInProgress_WhenStartIsCalled()
        {
            var task = new Task("Test task", null);

            bool result = task.Start();

            Assert.True(result);
            Assert.Equal(TaskItemStatus.InProgress, task.Status);
        }

        [Fact]
        public void Should_SetStartDate_WhenStartIsCalled()
        {
            var task = new Task("Test task", null);

            task.Start();

            Assert.NotNull(task.StartDate);
            var difference = DateTime.Now - task.StartDate.Value;
            Assert.True(difference.TotalSeconds < 1);
        }

        [Fact]
        public void Should_NotChangeStatus_ToInProgress_IfAlreadyInProgress()
        {
            var task = new Task("Test task", null);
            task.Start();

            bool result = task.Start();

            Assert.False(result);
            Assert.Equal(TaskItemStatus.InProgress, task.Status);
        }

        [Fact]
        public void Should_ChangeStatus_ToDone_WhenDoneIsCalledAndStatusIsInProgress()
        {
            var task = new Task("Test task", null);
            task.Start();

            bool result = task.Done();

            Assert.True(result);
            Assert.Equal(TaskItemStatus.Done, task.Status);
        }

        [Fact]
        public void Should_SetDoneDate_WhenDoneIsCalled()
        {
            var task = new Task("Test task", null);
            task.Start();

            task.Done();

            Assert.NotNull(task.DoneDate);
            var difference = DateTime.Now - task.DoneDate.Value;
            Assert.True(difference.TotalSeconds < 1);
        }

        [Fact]
        public void Should_NotChangeStatus_ToDone_IfStatusIsNotInProgress()
        {
            var task = new Task("Test task", null);

            bool result = task.Done();

            Assert.False(result);
            Assert.Equal(TaskItemStatus.ToDo, task.Status);
        }

        [Fact]
        public void Should_CalculateDuration_WhenStatusIsInProgress()
        {
            var task = new Task("Test task", null);
            task.Start();

            var duration = task.Duration;

            Assert.NotNull(duration);
        }

        [Fact]
        public void Should_ReturnNullDuration_WhenStatusIsTodo()
        {
            var task = new Task("Test task", null);

            var duration = task.Duration;

            Assert.Null(duration);
        }
    }
}