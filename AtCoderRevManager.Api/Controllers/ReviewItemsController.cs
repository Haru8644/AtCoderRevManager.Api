using AtCoderRevManager.Api.Data;
using AtCoderRevManager.Api.Dtos;
using AtCoderRevManager.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AtCoderRevManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reviewitems
        [HttpGet]
        public async Task<IActionResult> GetReviewItems()
        {
            var reviewItems = await _context.ReviewItems.ToListAsync();
            return Ok(reviewItems);
        }

        // GET: api/reviewitems/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewItemById(Guid id)
        {
            var reviewItem = await _context.ReviewItems.FindAsync(id);

            if (reviewItem == null)
            {
                return NotFound();
            }

            return Ok(reviewItem);
        }

        // POST: api/reviewitems
        [HttpPost]
        public async Task<IActionResult> CreateReviewItem([FromBody] CreateReviewItemDto reviewItemDto)
        {
            var problemExists = await _context.Problems.AnyAsync(p => p.Id == reviewItemDto.ProblemId);
            if (!problemExists)
            {
                return BadRequest("指定された問題IDは存在しません。");
            }

            var newReviewItem = new ReviewItem
            {
                Id = Guid.NewGuid(),
                ProblemId = reviewItemDto.ProblemId,
                Comment = reviewItemDto.Comment,
                CreatedAt = DateTime.UtcNow,
                NextReviewAt = DateTime.UtcNow.AddDays(1)
            };

            _context.ReviewItems.Add(newReviewItem);
            await _context.SaveChangesAsync();

            return Ok(newReviewItem);
        }

        // PUT: api/reviewitems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewItem(Guid id, [FromBody] UpdateReviewItemDto updateDto)
        {
            var itemToUpdate = await _context.ReviewItems.FindAsync(id);

            if (itemToUpdate == null)
            {
                return NotFound();
            }

            itemToUpdate.Comment = updateDto.Comment;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/reviewitems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewItem(Guid id)
        {
            var itemToDelete = await _context.ReviewItems.FindAsync(id);

            if (itemToDelete == null)
            {
                return NotFound();
            }

            _context.ReviewItems.Remove(itemToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}