using BasicBackend.Application.Dtos;
using BasicBackend.Application.Services;

namespace BasicBackend.Api
{
    public static class UsersEndpoints
    {
        public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users", async (CreateUserDto dto, IUserService svc) =>
            {
                var errs = Validate(dto);
                if (errs.Count > 0) return Results.BadRequest(new { errors = errs });

                var u = await svc.RegisterAsync(dto);
                return Results.Created($"/api/users/{u.Id}", new UserDto(u.Id, u.Name, u.Email));
            }).RequireRateLimiting("fixed");

            app.MapGet("/api/users/{id:long}", async (long id, IUserService svc) =>
            {
                var u = await svc.GetAsync(id);
                return u is null
                    ? Results.NotFound(new { error = "User not found" })
                    : Results.Ok(new UserDto(u.Id, u.Name, u.Email));
            }).RequireRateLimiting("fixed");

            app.MapDelete("/api/users/{id:long}", async (long id, IUserService svc) =>
            {
                var ok = await svc.DeleteAsync(id);
                return ok ? Results.NoContent() : Results.NotFound(new { error = "User not found" });
            }).RequireRateLimiting("fixed");

            return app;
        }

        static List<string> Validate(CreateUserDto dto)
        {
            var e = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Name)) e.Add("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@')) e.Add("Valid email is required.");
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6) e.Add("Password must be ≥ 6 chars.");
            return e;
        }
    }
}
