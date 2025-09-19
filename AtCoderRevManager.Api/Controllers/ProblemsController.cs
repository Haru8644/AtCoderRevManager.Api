using AtCoderRevManager.Api.Data;
using AtCoderRevManager.Api.Dtos;
using AtCoderRevManager.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AtCoderRevManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProblemsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetProblems()
        {
            var problems = await _context.Problems.ToListAsync();
            return Ok(problems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProblemById(string id)
        {
            var problem = await _context.Problems.FindAsync(id);
            if (problem == null)
            {
                return NotFound();
            }
            return Ok(problem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProblem([FromBody] CreateProblemDto problemDto)
        {
            var newProblem = new Problem
            {
                Id = problemDto.Id,
                ContestId = problemDto.ContestId,
                Title = problemDto.Title,
                Difficulty = problemDto.Difficulty
            };

            _context.Problems.Add(newProblem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProblemById), new { id = newProblem.Id }, newProblem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProblem(string id, [FromBody] UpdateProblemDto updateDto)
        {
            var problemToUpdate = await _context.Problems.FindAsync(id);
            if (problemToUpdate == null)
            {
                return NotFound();
            }

            problemToUpdate.Title = updateDto.Title;
            problemToUpdate.Difficulty = updateDto.Difficulty;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProblem(string id)
        {
            var problemToDelete = await _context.Problems.FindAsync(id);
            if (problemToDelete == null)
            {
                return NotFound();
            }

            _context.Problems.Remove(problemToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncProblems()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json,text/plain,*/*");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ja,en-US;q=0.9,en;q=0.8");

            try
            {
                var response = await client.GetAsync("https://kenkoooo.com/atcoder/resources/merged-problems.json");
                response.EnsureSuccessStatusCode();

                var problemsJson = await response.Content.ReadFromJsonAsync<List<Problem>>();
                if (problemsJson == null)
                {
                    return BadRequest("Failed to parse problems data.");
                }

                var existingProblemIdList = await _context.Problems.Select(p => p.Id).ToListAsync();
                var existingProblemIds = new HashSet<string>(existingProblemIdList);

                var newProblems = problemsJson
                    .Where(p => !existingProblemIds.Contains(p.Id))
                    .ToList();

                if (newProblems.Any())
                {
                    await _context.Problems.AddRangeAsync(newProblems);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    Message = "Sync completed successfully.",
                    NewProblemsAdded = newProblems.Count
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new
                {
                    Message = "Failed to fetch data from AtCoder Problems API.",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("sync-local")]
        public async Task<IActionResult> SyncProblemsFromLocalFile()
        {
            var filePath = @"C:\Users\haruk\Downloads\merged-problems.json";

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found at the specified path. Please download it first and update the path in the code.");
            }

            await using var fileStream = System.IO.File.OpenRead(filePath);
            var problemsJson = await JsonSerializer.DeserializeAsync<List<Problem>>(fileStream);

            if (problemsJson == null)
            {
                return BadRequest("Failed to parse problems data.");
            }

            var existingProblemIdList = await _context.Problems.Select(p => p.Id).ToListAsync();
            var existingProblemIds = new HashSet<string>(existingProblemIdList);

            var newProblems = problemsJson
                .Where(p => !existingProblemIds.Contains(p.Id))
                .ToList();

            if (newProblems.Any())
            {
                await _context.Problems.AddRangeAsync(newProblems);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                Message = "Sync from local file completed successfully.",
                NewProblemsAdded = newProblems.Count
            });
        }
    }
}