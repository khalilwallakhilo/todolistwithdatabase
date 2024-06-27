using FluentValidation;
using Microsoft.VisualBasic;
using ToDoList_ToDoListAPI.Models.dto;

namespace todolistwithdatabase
{
    public class ToDoListValidator: AbstractValidator<ToDoListDTO>
    {
        public ToDoListValidator() { 
            
            RuleFor(t => t.Title).NotEmpty().WithMessage("The title should not be left empty!");
            RuleFor(t => t.Description).NotEmpty().WithMessage("What is the meaning of life?");
            RuleFor(t => t.DueDate).NotEmpty().WithMessage("Due date is required.")
                                   .Must(IsValidDate).WithMessage("Invalid date. Date should follow the correct format and be before today.");
        }
        public static bool IsValidDate(string DueDate)
        {
            if (!DateTime.TryParse(DueDate, out DateTime tempObject))
            {
                return false; 
            }
            return tempObject.Date >= DateTime.Today;
        }
    }
}
