using System.Data.SqlClient;
using System.Text;
using Dapper;
using TaskManager.BusinessLogic;

namespace TaskManager
{
    public class Program
    {
        private const string ConnectionString = "Server=localhost,1433;Initial Catalog=TaskManager;User ID=sa;Password=P@ssw0rd;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

        private static TaskManagerService _taskManagerService = new TaskManagerService(new Repository(ConnectionString));
        private static int _createdBy = 1;

        public static async Task Main()
        {
            // Console.WriteLine(await TestDbAsync());
            string command;
            do
            {
                Console.WriteLine("0. Wyświetl użytkowników");
                Console.WriteLine("1. Dodaj zadanie");
                Console.WriteLine("2. Usuń zadanie");
                Console.WriteLine("3. Pokaż szczegóły zadania");
                Console.WriteLine("4. Wyświetl wszystkie zadania");
                Console.WriteLine("5. Wyświetl zadania wg statusu");
                Console.WriteLine("6. Szukaj zadania");
                Console.WriteLine("7. Zmień status zadania");
                Console.WriteLine("8. Przypisz zadanie");
                Console.WriteLine("9. Zakończ");

                command = Console.ReadLine().Trim();

                switch (command)
                {
                    case "0":
                        await DisplayAllUsersAsync();
                        break;
                    case "1":
                        await AddTaskAsync();
                        break;
                    case "2":
                        await RemoveTaskAsync();
                        break;
                    case "3":
                        await ShowTaskDetailsAsync();
                        break;
                    case "4":
                        await DisplayAllTasksAsync();
                        break;
                    case "5":
                        await DisplayAllTasksByStatusAsync();
                        break;
                    case "6":
                        await DisplaySearchedTasksAsync();
                        break;
                    case "7":
                        await UpdateTaskStatusAsync();
                        break;
                    case "8":
                        await AssignTaskAsync();
                        break;
                }
                Console.WriteLine("");
            } while (command != "9");
        }

        private static async Task DisplayAllUsersAsync()
        {
            var users = await _taskManagerService.GetAllUsersAsync();
            Console.WriteLine($"Jest {users.Length} użytkowników:");
            foreach (var user in users)
            {
                Console.WriteLine(user);
            }
        }

        private static async Task AddTaskAsync()
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

            var task = await _taskManagerService.AddAsync(description, _createdBy, dueDate);
            WriteLineSuccess($"Dodano zadanie: {task}");
        }

        private static async Task RemoveTaskAsync()
        {
            Console.WriteLine("Podaj identyfikator zadania do usunięcia:");
            int taskId;
            while (!int.TryParse(Console.ReadLine(), out taskId))
            {
                Console.WriteLine("Podaj identyfikator zadania do usunięcia:");
            }

            if (await _taskManagerService.RemoveAsync(taskId))
            {
                WriteLineSuccess($"Usunięto zadanie o numerze {taskId}");
            }
            else
            {
                WriteLineError($"Nie można usunąć zadania o numerze {taskId}");
            }
        }

        private static async Task ShowTaskDetailsAsync()
        {
            var taskId = ReadTaskId();
            var task = await _taskManagerService.GetAsync(taskId);
            if (task == null)
            {
                WriteLineError($"Nie można znaleźć zadania o numerze {taskId}");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(task.ToString());
            sb.AppendLine($"  Data utworzenia: {task.CreationDate}");
            sb.AppendLine($"  Utworzone przez: {task.CreatedBy}");
            sb.AppendLine($"  Przypisane do: {task.AssignedTo?.ToString() ?? ""}");
            sb.AppendLine($"  Data spodziewanego końca: {task.DueDate}");
            sb.AppendLine($"  Data startu: {task.StartDate}");
            sb.AppendLine($"  Data zakończenia: {task.DoneDate}");
            sb.AppendLine($"  Czas trwania: {task.Duration}");
            Console.WriteLine(sb);
        }

        private static async Task DisplayAllTasksAsync()
        {
            var tasks = await _taskManagerService.GetAllAsync();
            Console.WriteLine($"Jest {tasks.Length} zadań:");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }

        private static async Task DisplayAllTasksByStatusAsync()
        {
            var statuses = string.Join(", ", Enum.GetNames<TaskItemStatus>());
            Console.WriteLine($"Podaj status: {statuses}");
            TaskItemStatus itemStatus;
            while (!Enum.TryParse<TaskItemStatus>(Console.ReadLine(), true, out itemStatus))
            {
                Console.WriteLine($"Podaj status: {statuses}");
            }

            var tasks = await _taskManagerService.GetAllAsync(itemStatus);
            Console.WriteLine($"Jest {tasks.Length} zadań ({itemStatus}):");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }

        private static async Task DisplaySearchedTasksAsync()
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
            var tasks = await _taskManagerService.GetAllAsync(text);
            Console.WriteLine($"Znaleziono {tasks.Length} zadań:");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }

        private static async Task UpdateTaskStatusAsync()
        {
            var taskId = ReadTaskId();
            var statuses = string.Join(", ", Enum.GetNames<TaskItemStatus>());
            Console.WriteLine($"Podaj nowy status: {statuses}");
            TaskItemStatus itemStatus;
            while (!Enum.TryParse<TaskItemStatus>(Console.ReadLine(), true, out itemStatus))
            {
                Console.WriteLine($"Podaj nowy status: {statuses}");
            }

            if (await _taskManagerService.ChangeStatusAsync(taskId, itemStatus))
            {
                WriteLineSuccess($"Zmieniono status zadania o numerze {taskId}");
            }
            else
            {
                WriteLineError($"Nie można zmienić statusu zadania o numerze {taskId}");
            }
        }

        private static async Task AssignTaskAsync()
        {
            var taskId = ReadTaskId();
            var userId = ReadUserId();

            if (await _taskManagerService.AssignToAsync(taskId, userId))
            {
                WriteLineSuccess($"Przypisano zadanie o numerze {taskId} do użytkownika {userId}");
            }
            else
            {
                WriteLineError($"Nie można przypisać zadania o numerze {taskId} do użytkownika {userId}");
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

        private static int? ReadUserId()
        {
            while (true)
            {
                Console.WriteLine("Podaj identyfikator użytkownika lub pozostaw puste, aby odpiąć użytkownika od zadania:");
                var str = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(str))
                    return null;

                int userId;
                if (int.TryParse(str, out userId))
                    return userId;
            }
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

        private static async Task<string> TestDbAsync()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var sql = @"SELECT CONCAT(
    'Tabela TaskItems '
    , CASE WHEN OBJECT_ID('TaskItems', 'U') IS NOT NULL THEN 'istnieje' ELSE 'nieistnieje' END
    , CHAR(13)+CHAR(10)
    , CONCAT('Tabela Users ', CASE WHEN OBJECT_ID('Users', 'U') IS NOT NULL THEN 'istnieje' ELSE 'nieistnieje' END)
)";
                var result = await connection.QueryFirstAsync<string>(sql);
                return result;
            }
        }
    }
}