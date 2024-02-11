namespace TaskManager.BusinessLogic
{
    public class TaskItem
    {
        private User? _assignedTo = null;
        private User _createdBy;

        public int Id { get; }

        public string Description { get; set; }

        public DateTime CreationDate { get; }

        public DateTime? DueDate { get; set; }

        public DateTime? StartDate { get; private set; }

        public DateTime? DoneDate { get; private set; }

        public TimeSpan? Duration => StartDate != null ? (DoneDate ?? DateTime.Now) - StartDate.Value : null;

        public TaskItemStatus Status { get; private set; } = TaskItemStatus.ToDo;

        public User CreatedBy => _createdBy;

        public User? AssignedTo => _assignedTo;

        public TaskItem(int id, string description, User createdBy, DateTime? dueDate)
        {
            Id = id;
            Description = description;
            _createdBy = createdBy;
            CreationDate = DateTime.Now;
            DueDate = dueDate;
        }

        private TaskItem()
        {
        }

        public bool Start()
        {
            if (Status == TaskItemStatus.InProgress)
                return false;

            Status = TaskItemStatus.InProgress;
            StartDate = DateTime.Now;
            DoneDate = null;
            return true;
        }

        public bool Open()
        {
            if (Status == TaskItemStatus.ToDo)
                return false;

            Status = TaskItemStatus.ToDo;
            StartDate = null;
            DoneDate = null;
            return true;
        }

        public bool Done()
        {
            if (Status != TaskItemStatus.InProgress)
                return false;

            Status = TaskItemStatus.Done;
            DoneDate = DateTime.Now;
            return true;
        }

        public void AssignTo(User? assignedTo)
        {
            _assignedTo = assignedTo ?? null;
        }

        public override string ToString()
        {
            return AssignedTo != null ? $"{Id} - {Description} ({Status}) {AssignedTo.Name}" : $"{Id} - {Description} ({Status})";
        }
    }
}