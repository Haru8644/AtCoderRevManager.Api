using AtCoderRevManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AtCoderRevManager.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Problem> Problems { get; set; }
        public DbSet<ReviewItem> ReviewItems { get; set; }
    }
}