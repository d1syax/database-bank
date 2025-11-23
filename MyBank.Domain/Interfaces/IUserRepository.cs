using MyBank.Domain.Entities;

namespace MyBank.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    void Add(UserEntity user);
    void Update(UserEntity user);
}