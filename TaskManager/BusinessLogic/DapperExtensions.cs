using System.Reflection;

namespace TaskManager.BusinessLogic
{
    public static class DapperExtensions
    {
        public static TaskItem FixDapperMapping(this TaskItem taskItem, User createdBy, User assignedTo)
        {
            SetValueToObject(taskItem, "_createdBy", createdBy);
            SetValueToObject(taskItem, "_assignedTo", assignedTo);
            return taskItem;
        }

        private static void SetValueToObject(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(obj, value);
        }
    }
}