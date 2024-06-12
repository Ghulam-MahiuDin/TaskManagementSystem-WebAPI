using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Web.Data;
using TaskManagementSystem.Web.DTO;
using TaskManagementSystem.Web.Models;
using Task = TaskManagementSystem.Web.Models.Task;

namespace TaskManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public TasksController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }
        [HttpPost]
        public async Task<ActionResult> PostTask(TaskDto taskDto)
        {
           var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            else
            {

                var task = new Task
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Status = taskDto.Status,
                    UserId = userId,
                    DueDate = DateTime.Now,
                    //User = user


                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTask", new { id = task.Id }, task);
            }
        }
        /*   [HttpPost]
           public async Task<ActionResult> PostTask(TaskDto taskdto)
           {
               var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
               taskdto.UserId = userId;

               var Task = new Task
               {
                   Title = taskdto.Title,
                   Description = taskdto.Description,
                   Status = taskdto.Status,
                   UserId = userId
               };


               _context.Tasks.Add(Task);
               await _context.SaveChangesAsync();

               return CreatedAtAction("GetTask", new { id = Task.Id }, Task);
           }*/

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            if (task.UserId != userId)
            {
                return Unauthorized();
            }
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null)
            {
                return NotFound();
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}
