using Microsoft.VisualBasic;
using TodoAPI.DbModels;

namespace TodoAPI.Models

{
    public class TodoModel
    {
        public int Id { get; set; }
        public string Title {  get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }

        public static TodoModel todoEngine(Todo todo)
        {
            TodoModel newTodo = new TodoModel()
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                DueDate = todo.DueDate,
                Status = todo.Status.StatusOption,
            };

            return newTodo;
        }
    }
}
