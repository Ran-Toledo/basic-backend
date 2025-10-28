using BasicBackend.Domain;

namespace BasicBackend.Application.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(object dto);
        Task<User?> GetAsync(long id);
        Task<bool> DeleteAsync(long id);
    }
}
