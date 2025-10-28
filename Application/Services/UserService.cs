using BasicBackend.Application.Dtos;
using BasicBackend.Domain;
using BasicBackend.Infrastructure.Repositories;
using BasicBackend.Utilities;

namespace BasicBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<User> RegisterAsync(object dto)
        {
            var d = (CreateUserDto)dto;
            var existing = await _repo.GetByEmailAsync(d.Email);
            if (existing is not null) throw new BadHttpRequestException("Email already registered", 409);

            var salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.Hash(d.Password, salt);

            var user = new User
            {
                Name = d.Name.Trim(),
                Email = d.Email.Trim().ToLowerInvariant(),
                PasswordSalt = salt,
                PasswordHash = hash
            };

            return await _repo.AddAsync(user);
        }

        public Task<User?> GetAsync(long id)
        {
            return _repo.GetAsync(id);
        }

        public Task<bool> DeleteAsync(long id)
        {
            return _repo.DeleteAsync(id);
        }
    }
}
