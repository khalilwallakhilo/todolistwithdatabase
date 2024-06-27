using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList_ToDoListAPI.Models.dto
{
    public class ToDoListDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(80)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
       
        public string DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}