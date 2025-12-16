using VideoStreamCore.Application.Interfaces;
using VideoStreamCore.Domain.Entities;
using VideoStreamCore.Infrastructure.Data;

namespace VideoStreamCore.Infrastructure.Repository;

public class VideoRepository : IVideoRepository
{
    private readonly ApplicationDbContext _context;

    public VideoRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Video video)
    {
        await _context.Videos.AddAsync(video);
    }
    public async Task<Video?> GetByIdAsync(Guid id)
    {
        return await _context.Videos.FindAsync(id);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}