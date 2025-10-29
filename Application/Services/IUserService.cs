using BasicBackend.Domain;
using BasicBackend.Application.Dtos;

namespace BasicBackend.Application.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(object dto);
        Task<User?> GetAsync(long id);
        Task<bool> DeleteAsync(long id);
        Task<User?> UpdateAsync(long id, UpdateUserDto dto);
    }
}
