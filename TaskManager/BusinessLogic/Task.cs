namespace TaskManager.BusinessLogic
{
    public class Task
    {
        private static int _id;

        public int Id { get; }

        public string Description { get; set; }

        public DateTime CreationDate { get; }

        public DateTime? DueDate { get; set; }

        public DateTime? StartDate { get; private set; }

        public DateTime? DoneDate { get; private set; }

        public TimeSpan? Duration => StartDate != null ? (DoneDate ?? DateTime.Now) - StartDate.Value : null;

        public TaskStatus Status { get; private set; } = TaskStatus.ToDo;

        public Task(string description, DateTime? dueDate)
        {
            Id = ++_id;
            Description = description;
            CreationDate = DateTime.Now;
            DueDate = dueDate;
        }

        public bool Start()
        {
            if (Status == TaskStatus.InProgress)
                return false;

            Status = TaskStatus.InProgress;
            StartDate = DateTime.Now;
            DoneDate = null;
            return true;
        }

        public bool Open()
        {
            if (Status == TaskStatus.ToDo)
                return false;

            Status = TaskStatus.ToDo;
            StartDate = null;
            DoneDate = null;
            return true;
        }

        public bool Done()
        {
            if (Status != TaskStatus.InProgress)
                return false;

            Status = TaskStatus.Done;
            DoneDate = DateTime.Now;
            return true;
        }

        public override string ToString()
        {
            return $"{Id} - {Description} ({Status})";
        }
    }
}