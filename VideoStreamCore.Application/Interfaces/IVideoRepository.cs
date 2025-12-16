using VideoStreamCore.Domain.Entities;

namespace VideoStreamCore.Application.Interfaces;

public interface IVideoRepository
{
    Task AddAsync(Video video);
    Task<Video?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}