using Lct2023.Api.Definitions.Dto;

namespace Lct2023.Api.Services.Interfaces;

public interface IRatingService
{
    Task<int> GetRatingAsync(int userId);

    Task AddRatingAsync(int userId, int amount);
}