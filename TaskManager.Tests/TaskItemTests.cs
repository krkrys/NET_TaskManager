using TaskManager.BusinessLogic;

namespace TaskManager.Tests
{
    public class TaskItemTests
    {
        private User _createdBy = new User(1, "Ja");

        [Fact]
        public void Should_CreateTask_WithAutoIncrementedId()
        {
            var task1 = new TaskItem(1, "Test task 1", _createdBy, null);
            var task2 = new TaskItem(2, "Test task 2", _createdBy, null);

            Assert.True(task1.Id > 0);
            Assert.True(task1.Id < task2.Id);
        }

        [Fact]
        public void Should_SetCreationDate_WhenCreatingTask()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);
            var difference = DateTime.Now - task.CreationDate;

            Assert.True(difference.TotalSeconds < 1);
        }

        [Fact]
        public void Should_SetDueDate_WhenProvided()
        {
            var dueDate = DateTime.Now.AddDays(7);
            var task = new TaskItem(1, "Test task", _createdBy, dueDate);

            Assert.Equal(dueDate, task.DueDate);
        }

        [Fact]
        public void Should_SetStatusToTodo_WhenTaskIsCreated()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);

            Assert.Equal(TaskItemStatus.ToDo, task.Status);
        }

        [Fact]
        public void Should_ChangeStatus_ToInProgress_WhenStartIsCalled()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);

            bool result = task.Start();

            Assert.True(result);
            Assert.Equal(TaskItemStatus.InProgress, task.Status);
        }

        [Fact]
        public void Should_SetStartDate_WhenStartIsCalled()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);

            task.Start();

            Assert.NotNull(task.StartDate);
            var difference = DateTime.Now - task.StartDate.Value;
            Assert.True(difference.TotalSeconds < 1);
        }

        [Fact]
        public void Should_NotChangeStatus_ToInProgress_IfAlreadyInProgress()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);
            task.Start();

            bool result = task.Start();

            Assert.False(result);
            Assert.Equal(TaskItemStatus.InProgress, task.Status);
        }

        [Fact]
        public void Should_ChangeStatus_ToDone_WhenDoneIsCalledAndStatusIsInProgress()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);
            task.Start();

            bool result = task.Done();

            Assert.True(result);
            Assert.Equal(TaskItemStatus.Done, task.Status);
        }

        [Fact]
        public void Should_SetDoneDate_WhenDoneIsCalled()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);
            task.Start();

            task.Done();

            Assert.NotNull(task.DoneDate);
            var difference = DateTime.Now - task.DoneDate.Value;
            Assert.True(difference.TotalSeconds < 1);
        }

        [Fact]
        public void Should_NotChangeStatus_ToDone_IfStatusIsNotInProgress()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);

            bool result = task.Done();

            Assert.False(result);
            Assert.Equal(TaskItemStatus.ToDo, task.Status);
        }

        [Fact]
        public void Should_CalculateDuration_WhenStatusIsInProgress()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);
            task.Start();

            var duration = task.Duration;

            Assert.NotNull(duration);
        }

        [Fact]
        public void Should_ReturnNullDuration_WhenStatusIsTodo()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);

            var duration = task.Duration;

            Assert.Null(duration);
        }

        [Fact]
        public void Should_AssignTo_User()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);

            task.AssignTo(_createdBy);

            Assert.Equal(_createdBy, task.AssignedTo);
        }

        [Fact]
        public void Should_Unassign_User_When_Null_Passed()
        {
            var task = new TaskItem(1, "Test task", _createdBy, null);
            task.AssignTo(_createdBy);

            task.AssignTo(null);

            Assert.Null(task.AssignedTo);
        }
    }
}