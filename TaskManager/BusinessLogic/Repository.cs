using System.Data.SqlClient;
using System.Reflection;
using Dapper;

namespace TaskManager.BusinessLogic
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User[]> GetAllUsersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var users = await connection.QueryAsync<User>("SELECT * FROM Users");
                return users.ToArray();
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var user = await connection.QueryFirstAsync<User>("SELECT * FROM Users WHERE Id = @Id", new {Id = userId});
                return user;
            }
        }

        public async Task<int> CreateTaskItemAsync(TaskItem newTaskItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = 
                    "INSERT INTO TaskItems (Description, CreatedById, CreationDate, Status, DueDate) " +
                    "VALUES (@Description, @CreatedById, @CreationDate, @Status, @DueDate);" + 
                    "SELECT SCOPE_IDENTITY();";
                var newTask = new
                {
                    Description = newTaskItem.Description,
                    CreatedById = newTaskItem.CreatedBy.Id,
                    CreationDate = newTaskItem.CreationDate,
                    Status = newTaskItem.Status,
                    DueDate = newTaskItem.DueDate
                };
                var id = await connection.ExecuteScalarAsync<int>(sql, newTask);
                return id;
            }
        }
        
        public async Task<bool> UpdateTaskItemAsync(TaskItem newTaskItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql =
                    "UPDATE TaskItems " +
                    "SET Status = @Status, StartDate = @StartDate, DoneDate = @DoneDate, AssignedToId = @AssignedTo " +
                    "WHERE Id = @Id";

                var updateTask = new
                {
                    Id = newTaskItem.Id,
                    Status = newTaskItem.Status,
                    StartDate = newTaskItem.StartDate,
                    DoneDate = newTaskItem.DoneDate,
                    AssignedTo = newTaskItem.AssignedTo?.Id
                };
                var result = await connection.ExecuteAsync(sql, updateTask);
                return result == 1;
            }
        }

        public async Task<bool> DeleteTaskItemAsync(int taskItemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM TaskItems WHERE Id = @Id";
                var result = await connection.ExecuteAsync(sql, new {Id = taskItemId});
                return result == 1;
            }
        }

        public async Task<TaskItem> GetTaskItemByIdAsync(int taskItemId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql =
                    "SELECT task.*, createdBy.*, assignedTo.* FROM TaskItems task " +
                    "INNER JOIN Users createdBy ON createdBy.Id = task.CreatedById " +
                    "LEFT JOIN Users assignedTo ON assignedTo.Id = task.AssignedToId " +
                    "WHERE task.Id = @TaskId";
                var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
                    sql,
                    (task, createdBy, assignedTo) =>
                    {
                        return task.FixDapperMapping(createdBy, assignedTo);
                    },
                    new {TaskId = taskItemId});
                return tasks.FirstOrDefault();
            }
        }

        public async Task<TaskItem[]> GetAllTaskItemsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql =
                    "SELECT task.*, createdBy.*, assignedTo.* FROM TaskItems task " +
                    "INNER JOIN Users createdBy ON createdBy.Id = task.CreatedById " +
                    "LEFT JOIN Users assignedTo ON assignedTo.Id = task.AssignedToId";
                var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
                    sql,
                    (task, createdBy, assignedTo) =>
                    {
                        return task.FixDapperMapping(createdBy, assignedTo);
                    });
                return tasks.ToArray();
            }
        }

        public async Task<TaskItem[]> GetTaskItemsByStatusAsync(TaskItemStatus status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql =
                    "SELECT task.*, createdBy.*, assignedTo.* FROM TaskItems task " +
                    "INNER JOIN Users createdBy ON createdBy.Id = task.CreatedById " +
                    "LEFT JOIN Users assignedTo ON assignedTo.Id = task.AssignedToId " +
                    "WHERE Task.Status = @TaskStatus";
                var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
                    sql,
                    (task, createdBy, assignedTo) =>
                    {
                        return task.FixDapperMapping(createdBy, assignedTo);
                    },
                    new {TaskStatus = status});
                return tasks.ToArray();
            }
        }

        public async Task<TaskItem[]> GetTaskItemsByDescriptionAsync(string description)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql =
                    "SELECT task.*, createdBy.*, assignedTo.* FROM TaskItems task " +
                    "INNER JOIN Users createdBy ON createdBy.Id = task.CreatedById " +
                    "LEFT JOIN Users assignedTo ON assignedTo.Id = task.AssignedToId " +
                    "WHERE Task.Description LIKE @Description";
                var tasks = await connection.QueryAsync<TaskItem, User, User, TaskItem>(
                    sql,
                    (task, createdBy, assignedTo) =>
                    {
                        return task.FixDapperMapping(createdBy, assignedTo);
                    },
                    new {Description = $"%{description}%"});
                return tasks.ToArray();
            }
        }
    }
}