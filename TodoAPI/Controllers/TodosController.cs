using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net.NetworkInformation;
using TodoAPI.Models;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TodoAPI.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]


    public class TodosController : ControllerBase
    {
        private TodoContext dB = new TodoContext();
        private static List<TodoModel> todos = new List<TodoModel>()
        {
                new TodoModel()
                {
                    Id = 1,
                    Title = "Test",
                    Description = "Test",
                    DueDate = DateTime.UtcNow,
                    Status = "Complete"
                },
                new TodoModel()
                {
                    Id = 2,
                Title = "Listen to music",
                Description = "Take a break and listen to some good music.",
                 DueDate = DateTime.UtcNow,
                Status = "In Progress"
                },
        };
        [Authorize]
        [HttpGet(Name = "GetTodos")]
        public IEnumerable<TodoModel> Get() //[FromQuery]string username
        {
            var username = User.Identity?.Name;
            IEnumerable<TodoModel> todosFromDatabase = new List<TodoModel>();
            //IEnumerable<Todo> todosdb = dB.Todos.Include(t => t.Status).ToList();
            IEnumerable<Todo> todosdb = dB.Todos.Where(t => t.User.Username == username).Include(t => t.Status).ToList();
            todosFromDatabase = todosdb.Select(t => TodoModel.todoEngine(t));
            return todosFromDatabase;
        }

        [Authorize]
        [HttpPost(Name = "CreateTodo")]
        public ActionResult Create(TodoModel newTodo/*, [FromQuery]string username*/)
        {
            var username = User.Identity?.Name;
            
            if (newTodo == null)
            {
                return BadRequest();
            }
            var status = dB.TodoStatuses.FirstOrDefault(s => s.StatusOption == newTodo.Status);
            var user = dB.Users.FirstOrDefault(u => u.Username == username);
            
            if (status == null)
            {
                return BadRequest("Invalid Status");
            }
            if (user == null)
            {
                return BadRequest("Invalid User");
            }

            Todo todo = new Todo
            {
                Title = newTodo.Title,
                Description = newTodo.Description,
                DueDate = newTodo.DueDate,
                Status = status,   
                User = user
            };

            dB.Todos.Add(todo);
            dB.SaveChanges();

            return Ok(TodoModel.todoEngine(todo));

            //newTodo.Id = todos.Count > 0 ? todos.Max(todo => todo.Id) + 1 : 1;
            //todos.Add(newTodo);
            //return Ok(newTodo.Id);

        }

        [Authorize]
        [HttpPut("{id}", Name = "EditTodo")]
        public ActionResult Update(int id, [FromBody] TodoModel updatedTodo)
        {
            var username = User.Identity?.Name;
            if (updatedTodo == null || string.IsNullOrWhiteSpace(updatedTodo.Status))
                return BadRequest("Status is required.");

            var todo = dB.Todos.Find(id);
            if (todo == null)
                return NotFound();

         
            var status = dB.TodoStatuses.FirstOrDefault(s => s.StatusOption == updatedTodo.Status);
            var user = dB.Users.FirstOrDefault(u => u.Username == username);
            if (status == null)
                return BadRequest($"Invalid Status: {updatedTodo.Status}");
            if (user == null)
            {
                return BadRequest($"Invalid User: {username}");
            }

            todo.Title = updatedTodo.Title;
            todo.Description = updatedTodo.Description;
            todo.DueDate = updatedTodo.DueDate;
            todo.Status = status;
            todo.User = user;

            dB.SaveChanges();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}", Name = "DeleteTodo")]

        public ActionResult Delete(int id)
        {
            
            var todo = dB.Todos.Find(id);
            if (todo == null)
            {
                return NotFound();
            }
            dB.Todos.Remove(todo);
            dB.SaveChanges();

            return NoContent();
        }
    }
}
