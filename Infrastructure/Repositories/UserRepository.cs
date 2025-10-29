using Microsoft.EntityFrameworkCore;
using BasicBackend.Domain;
using BasicBackend.Infrastructure.Data;

namespace BasicBackend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDb _db;

        public UserRepository(AppDb db)
        {
            _db = db;
        }

        public Task<User?> GetAsync(long id)
        {
            return _db.Users.FindAsync(id).AsTask();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u is null) return false;
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
