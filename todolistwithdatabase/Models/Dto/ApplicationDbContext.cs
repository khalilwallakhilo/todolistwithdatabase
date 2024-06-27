using Microsoft.EntityFrameworkCore;
using ToDoList_ToDoListAPI.Models;

namespace todolistwithdatabase.Models.Dto
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<ToDoList> Lists { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoList>().HasData(
                new ToDoList()
                {
                    Id = 1,
                    Description = "hah",
                    Title = "Lorem Epsum",
                    IsCompleted = false,
                    DueDate = "2024-10-5"
                }
                );


            
        }
    }
}
