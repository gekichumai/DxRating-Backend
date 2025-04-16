using DxRating.Database;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Utils;
using Microsoft.EntityFrameworkCore;

namespace DxRating.Services.Authentication.Services;

public class UserService
{
    private readonly DxDbContext _dbContext;

    public UserService(DxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserAsync(Guid id)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await GetUserQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> CreateUserFromLocalAsync(string email, string password)
    {
        var hashedPassword = SecurityUtils.HashPassword(password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = email,
            Password = hashedPassword,
            EmailConfirmed = false
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public IQueryable<User> GetUserQueryable()
    {
        var queryable = _dbContext.Users.AsQueryable();
        return queryable;
    }
}
