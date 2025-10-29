using BasicBackend.Domain;

namespace BasicBackend.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetAsync(long id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<bool> DeleteAsync(long id);
        Task<int> SaveChangesAsync();
    }
}
