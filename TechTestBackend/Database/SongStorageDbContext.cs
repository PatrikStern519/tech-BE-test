using Microsoft.EntityFrameworkCore;
using TechTestBackend.Models;

namespace TechTestBackend.Database;

public class SongStorageDbContext : DbContext
{
    public SongStorageDbContext(DbContextOptions<SongStorageDbContext> options)
        : base(options)
    {
    }

    public DbSet<SpotifySong> Songs { get; set; }
}