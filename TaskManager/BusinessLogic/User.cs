namespace TaskManager.BusinessLogic
{
    public class User
    {
        public int Id { get; }
        public string Name { get; }
        public List<TaskItem> Tasks { get; }

        private User() { }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
            Tasks = new List<TaskItem>();
        }

        public override string ToString() => $"{Id}. {Name}";
    }
}