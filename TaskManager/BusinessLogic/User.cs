using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.BusinessLogic
{
    public class User
    {
        public int Id { get; }
        public string Name { get; set; }
        public List<TaskItem> UserTasks { get; set; }

        private User()
        {
        }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
            UserTasks = new List<TaskItem>();
        }
        public override string ToString()
        {
            return $"{Id}. {Name}";
        }
    }
}
