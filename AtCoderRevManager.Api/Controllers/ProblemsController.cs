using AtCoderRevManager.Api.Data;
using AtCoderRevManager.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AtCoderRevManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProblemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProblems()
        {
            var problems = _context.Problems.ToList();
            return Ok(problems);
        }

        [HttpPost]
        public IActionResult CreateProblem([FromBody] CreateProblemDto problemDto)
        {
            var newProblem = new Models.Problem
            {
                Id = problemDto.Id,
                ContestId = problemDto.ContestId,
                Title = problemDto.Title,
                Difficulty = problemDto.Difficulty
            };

            _context.Problems.Add(newProblem);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetProblems), new { id = newProblem.Id }, newProblem);
        }

        [HttpGet("{id}")]
        public IActionResult GetProblemById(string id)
        {
            var problem = _context.Problems.Find(id);
            if (problem == null)
            {
                return NotFound();
            }
            return Ok(problem);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProblem(string id, [FromBody] UpdateProblemDto updateDto)
        {
            var problemToUpdate = _context.Problems.Find(id);
            if (problemToUpdate == null)
            {
                return NotFound();
            }

            problemToUpdate.Title = updateDto.Title;
            problemToUpdate.Difficulty = updateDto.Difficulty;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProblem(string id)
        {
            var problemToDelete = _context.Problems.Find(id);
            if (problemToDelete == null)
            {
                return NotFound();
            }

            _context.Problems.Remove(problemToDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
