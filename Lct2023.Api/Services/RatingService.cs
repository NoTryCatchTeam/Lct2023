using Lct2023.Api.Dal;
using Lct2023.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Services;

public class RatingService : IRatingService
{
    private readonly NpgsqlDbContext _dbContext;

    public RatingService(NpgsqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRatingAsync(int userId, int amount)
    {
        var userInfo = await _dbContext.UserInfos.Where(x => x.User.Id == userId).FirstOrDefaultAsync();
        if (userInfo is null)
        {
            userInfo = new UserInfo()
            {
                ExtendedIdentityUserId = userId
            };
            userInfo.Rating = userInfo.Rating + amount;
            _dbContext.UserInfos.Add(userInfo);
        }
        else
        {
            userInfo.Rating = userInfo.Rating + amount;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> GetRatingAsync(int userId)
    {
        var userInfo = await _dbContext.UserInfos.Where(x => x.User.Id == userId).FirstOrDefaultAsync();
        return userInfo?.Rating ?? 0;
    }
}