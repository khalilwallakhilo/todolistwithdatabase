using System.Runtime.Serialization;
using ToDoList_ToDoListAPI.Models.dto;

namespace ToDoList_ToDoListAPI.Data
{
    public static class ToDoListStore
    {
        public static List<ToDoListDTO> list = new List<ToDoListDTO>
            {
                new ToDoListDTO { Id = 1, Title = "Groceries", Description = "Get Vegetables", DueDate = "2024-10-10", IsCompleted = false },
                new ToDoListDTO { Id = 2, Title = "Bank", Description = "Get Money", DueDate = "2024-08-15" , IsCompleted = false }
            };
    }
}
