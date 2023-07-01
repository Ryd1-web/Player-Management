using Microsoft.EntityFrameworkCore;

namespace Player_Management.Models
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Player> Players { get; set; }
	}
}
