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

        public async Task<User?> UpdateAsync(long id, UpdateUserDto dto)
        {
            var target = await _repo.GetAsync(id);
            if (target is null) return null;

            var other = await _repo.GetByEmailAsync(dto.Email);
            if (other is not null && other.Id != id) throw new BadHttpRequestException("Email already registered", 409);

            target.Name = dto.Name.Trim();
            target.Email = dto.Email.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var salt = PasswordHasher.GenerateSalt();
                var hash = PasswordHasher.Hash(dto.Password, salt);
                target.PasswordSalt = salt;
                target.PasswordHash = hash;
            }

            await _repo.SaveChangesAsync();
            return target;
        }
    }
}
