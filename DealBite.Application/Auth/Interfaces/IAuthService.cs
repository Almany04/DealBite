

namespace DealBite.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error, Guid? UserId, string? Email)> RegisterAsync(string email, string password, string displayName);
        Task<(bool Success, string? Error, Guid? UserId, string? Email)> LoginAsync(string email, string password);
    }
}
