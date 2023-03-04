using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<Product> Products { get; set; }
  public DbSet<User> Users { get; set; }
}
