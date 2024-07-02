using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList_ToDoListAPI.Data;
using ToDoList_ToDoListAPI.Models;
using ToDoList_ToDoListAPI.Models.dto;
using todolistwithdatabase.Models.Dto;

namespace TDL_TDLAPI.Controllers
{

    [Route("api/ToDoListAPI")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    //I am authorizing all the endpoints in this controller 
    public class ToDoListAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ToDoListAPIController(ApplicationDbContext db)
        {
            _db = db;

        }

        [HttpGet, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //Getting to do lists could be done anonymously 
        public ActionResult<IEnumerable<ToDoList>> GetToDoLists(int page = 1, int pageSize = 10, string search = null)
        {
            if (page <= 0 || pageSize <= 0)
            {
                ModelState.AddModelError("ERROR - ", "Page/Page Size is invalid.");
                return NotFound(ModelState);
            }
            else
            {
                var query = _db.Lists.AsQueryable();
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(list => list.Title.Contains(search) || list.Description.Contains(search));
                }

                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var tasksperpage = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    TasksPerPage = tasksperpage
                });
            }
        }

        [HttpGet("{isCompleted?}"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<ToDoListDTO> GetToDoList(string title = null, bool? isCompleted =null)
        {

            IQueryable<ToDoList> query = _db.Lists;

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(t => t.Title.Contains(title));
            }
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            var tdl = query.FirstOrDefault();

            if (tdl == null)
            {
                ModelState.AddModelError("ERROR - ", "No task found matching the criteria.");
                return NotFound(ModelState);
            }

            return Ok(tdl);

        }
        public static bool IsValidDate(string DueDate)
        {
            if (!DateTime.TryParse(DueDate, out DateTime tempObject))
            {
                return false;
            }
            return tempObject.Date >= DateTime.Today;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<ToDoListDTO> AddTask([FromBody] ToDoListDTO dto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (_db.Lists.FirstOrDefault(u => u.Title.ToLower() == dto.Title.ToLower()) != null)
            {
                ModelState.AddModelError("ERROR - ", "This task has already been recorded!");
                return BadRequest(ModelState);
            }
            if (!IsValidDate(dto.DueDate))
            {
                ModelState.AddModelError("ERROR - ", "This date does not exist");
                return BadRequest(ModelState);
            }


            if (dto == null)
            {
                return BadRequest(dto);
            }
            if (dto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            ToDoList model = new()
            {
                Description = dto.Description,
                Title = dto.Title,
                Id = dto.Id,
                DueDate = dto.DueDate,
                IsCompleted = dto.IsCompleted,
            };


            _db.Lists.Add(model);
            _db.SaveChanges();


            return CreatedAtRoute("GetToDoList", new { id = dto.Id }, dto);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteList")]
        public IActionResult DeleteTask(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var dto = _db.Lists.FirstOrDefault(u => u.Id == id);
            if (dto == null)
            {
                return NotFound();
            }
            _db.Lists.Remove(dto);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateList")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateTask(int id, [FromBody] ToDoListDTO dto)
        {
            if (dto == null || id != dto.Id)
            {
                return BadRequest();
            }
            ToDoList model = new()
            {
                Description = dto.Description,
                Title = dto.Title,
                Id = dto.Id,
                DueDate = dto.DueDate,
                IsCompleted = dto.IsCompleted,
            };

            _db.Lists.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdateCompletion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCompletion(int id, JsonPatchDocument<ToDoListDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var l = _db.Lists.AsNoTracking().FirstOrDefault(u => u.Id == id);

            ToDoListDTO model = new()
            {
                Description = l.Description,
                Title = l.Title,
                Id = l.Id,
                DueDate = l.DueDate,
                IsCompleted = l.IsCompleted,
            };
            if (l == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(model, ModelState);

            ToDoList ad = new()
            {
                Description = model.Description,
                Title = model.Title,
                Id = model.Id,
                DueDate = model.DueDate,
                IsCompleted = model.IsCompleted,
            };
            _db.Lists.Update(ad);
            _db.SaveChanges();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}