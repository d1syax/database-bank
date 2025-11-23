using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<UserEntity>> GetAllAsync(CancellationToken ct = default);
    Task<bool> IsEmailExists(string email, CancellationToken cancellationToken = default);
    Task AddAsync(UserEntity user, CancellationToken ct = default);
    Task UpdateAsync(UserEntity user, CancellationToken ct = default);
    Task DeleteAsync(UserEntity user, CancellationToken ct = default);
}