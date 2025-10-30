using Bogus;
using BasicBackend.Domain;
using BasicBackend.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BasicBackend.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDb db, IConfiguration cfg)
        {
            await db.Database.EnsureCreatedAsync();

            var enabled = cfg.GetValue<bool>("Seed:Enabled");
            if (!enabled) return;

            var ensure = cfg.GetSection("Seed:EnsureEmails").Get<string[]>() ?? Array.Empty<string>();
            foreach (var email in ensure)
            {
                await UpsertUserByEmailAsync(db, email, Capitalize(email.Split('@')[0]), "Password1!");
            }

            var fakeCount = cfg.GetValue<int>("Seed:FakeUsers");
            if (fakeCount > 0)
            {
                var faker = new Faker<User>()
                    .StrictMode(true)
                    .RuleFor(u => u.Id, _ => 0)
                    .RuleFor(u => u.Name, f => f.Name.FirstName())
                    .RuleFor(u => u.Email, f => f.Internet.Email().ToLowerInvariant())
                    .RuleFor(u => u.PasswordSalt, _ => "")
                    .RuleFor(u => u.PasswordHash, _ => "")
                    .RuleFor(u => u.CreatedUtc, _ => DateTime.UtcNow);

                var users = faker.Generate(fakeCount);
                foreach (var u in users)
                {
                    var salt = PasswordHasher.GenerateSalt();
                    var hash = PasswordHasher.Hash("Password1!", salt);
                    u.PasswordSalt = salt;
                    u.PasswordHash = hash;

                    var exists = await db.Users.AnyAsync(x => x.Email == u.Email);
                    if (!exists) db.Users.Add(u);
                }

                await db.SaveChangesAsync();
            }
        }

        static async Task UpsertUserByEmailAsync(AppDb db, string email, string name, string password)
        {
            var u = await db.Users.FirstOrDefaultAsync(x => x.Email == email.ToLowerInvariant());
            if (u is null)
            {
                var salt = PasswordHasher.GenerateSalt();
                var hash = PasswordHasher.Hash(password, salt);

                db.Users.Add(new User
                {
                    Name = name,
                    Email = email.ToLowerInvariant(),
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    CreatedUtc = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
            else
            {
                var changed = false;
                var nm = name.Trim();
                if (u.Name != nm) { u.Name = nm; changed = true; }
                if (changed) await db.SaveChangesAsync();
            }
        }

        static string Capitalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            if (s.Length == 1) return s.ToUpperInvariant();
            return char.ToUpperInvariant(s[0]) + s[1..].ToLowerInvariant();
        }
    }
}
