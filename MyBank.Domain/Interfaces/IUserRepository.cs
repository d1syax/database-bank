using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> IsEmailExists(string email, CancellationToken ct = default);
    Task AddAsync(UserEntity user, CancellationToken ct = default);
    Task UpdateAsync(UserEntity user, CancellationToken ct = default);
    Task DeleteAsync(UserEntity user, CancellationToken ct = default);
}