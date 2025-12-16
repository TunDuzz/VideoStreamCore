using Microsoft.EntityFrameworkCore;
using VideoStreamCore.Domain.Entities;

namespace VideoStreamCore.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Video> Videos { get; set; }
}