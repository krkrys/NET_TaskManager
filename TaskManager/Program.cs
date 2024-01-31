using System.Text;
using TaskManager.BusinessLogic;
//TaskStatus już istnieje w przestrzeni System.Threading.Tasks, która jest automatycznie importowana.
//Musimy rozwiązać konflikt nazw stosując alias.
using TaskStatus = TaskManager.BusinessLogic.TaskStatus;

namespace TaskManager
{
    public class Program
    {
        private static TaskManagerService _taskManagerService = new TaskManagerService();

        public static void Main()
        {
            string command;
            do
            {
                Console.WriteLine("1. Dodaj zadanie");
                Console.WriteLine("2. Usuń zadanie");
                Console.WriteLine("3. Pokaż szczegóły zadania");
                Console.WriteLine("4. Wyświetl wszystkie zadania");
                Console.WriteLine("5. Wyświetl zadania wg statusu");
                Console.WriteLine("6. Szukaj zadania");
                Console.WriteLine("7. Zmień status zadania");
                Console.WriteLine("8. Zakończ");

                command = Console.ReadLine().Trim();

                switch (command)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        RemoveTask();
                        break;
                    case "3":
                        ShowTaskDetails();
                        break;
                    case "4":
                        DisplayAllTasks();
                        break;
                    case "5":
                        DisplayAllTasksByStatus();
                        break;
                    case "6":
                        DisplaySearchedTasks();
                        break;
                    case "7":
                        UpdateTaskStatus();
                        break;
                }
                Console.WriteLine("");
            } while (command != "8");
        }

        private static void AddTask()
        {
            Console.WriteLine("Podaj opis zadania:");
            var description = Console.ReadLine();

            Console.WriteLine("Podaj datę wykonania (lub pozostaw puste):");
            DateTime? dueDate = null;
            DateTime date;
            if (DateTime.TryParse(Console.ReadLine(), out date))
            {
                dueDate = date;
            }

            var task = _taskManagerService.Add(description, dueDate);
            WriteLineSuccess($"Dodano zadanie: {task}");
        }

        private static void RemoveTask()
        {
            Console.WriteLine("Podaj identyfikator zadania do usunięcia:");
            int taskId;
            while (!int.TryParse(Console.ReadLine(), out taskId))
            {
                Console.WriteLine("Podaj identyfikator zadania do usunięcia:");
            }

            if (_taskManagerService.Remove(taskId))
            {
                WriteLineSuccess($"Usunięto zadanie o numerze {taskId}");
            }
            else
            {
                WriteLineError($"Nie można usunąć zadania o numerze {taskId}");
            }
        }

        private static void ShowTaskDetails()
        {
            var taskId = ReadTaskId();
            var task = _taskManagerService.Get(taskId);
            if (task == null)
            {
                WriteLineError($"Nie można znaleźć zadania o numerze {taskId}");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(task.ToString());
            sb.AppendLine($"  Data utworzenia: {task.CreationDate}");
            sb.AppendLine($"  Data spodziewanego końca: {task.DueDate}");
            sb.AppendLine($"  Data startu: {task.StartDate}");
            sb.AppendLine($"  Data zakończenia: {task.DoneDate}");
            sb.AppendLine($"  Czas trwania: {task.Duration}");
            Console.WriteLine(sb);
        }

        private static void DisplayAllTasks()
        {
            var tasks = _taskManagerService.GetAll();
            Console.WriteLine($"Masz {tasks.Length} zadań:");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }

        private static void DisplayAllTasksByStatus()
        {
            var statuses = string.Join(", ", Enum.GetNames<TaskStatus>());
            Console.WriteLine($"Podaj status: {statuses}");
            TaskStatus status;
            while (!Enum.TryParse<TaskStatus>(Console.ReadLine(), true, out status))
            {
                Console.WriteLine($"Podaj status: {statuses}");
            }

            var tasks = _taskManagerService.GetAll(status);
            Console.WriteLine($"Masz {tasks.Length} zadań ({status}):");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }

        private static void DisplaySearchedTasks()
        {
            Console.WriteLine($"Wyszukaj zadania o treści (możesz podać fragment):");
            string text;
            while (true)
            {
                text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine($"Wyszukaj zadania o treści (możesz podać fragment):");
                    continue;
                }
                break;
            }
            var tasks = _taskManagerService.GetAll(text);
            Console.WriteLine($"Znaleziono {tasks.Length} zadań:");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }

        private static void UpdateTaskStatus()
        {
            var taskId = ReadTaskId();
            var statuses = string.Join(", ", Enum.GetNames<TaskStatus>());
            Console.WriteLine($"Podaj nowy status: {statuses}");
            TaskStatus status;
            while (!Enum.TryParse<TaskStatus>(Console.ReadLine(), true, out status))
            {
                Console.WriteLine($"Podaj nowy status: {statuses}");
            }

            if (_taskManagerService.ChangeStatus(taskId, status))
            {
                WriteLineSuccess($"Zmieniono status zadania o numerze {taskId}");
            }
            else
            {
                WriteLineError($"Nie można zmienić statusu zadania o numerze {taskId}");
            }
        }

        private static int ReadTaskId()
        {
            Console.WriteLine("Podaj identyfikator zadania:");
            int taskId;
            while (!int.TryParse(Console.ReadLine(), out taskId))
            {
                Console.WriteLine("Podaj identyfikator zadania:");
            }
            return taskId;
        }

        private static void WriteLineSuccess(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void WriteLineError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}