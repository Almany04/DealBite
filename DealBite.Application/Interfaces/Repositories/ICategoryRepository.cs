using DealBite.Domain.Entities;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface ICategoryRepository:IGenericRepository<Category>
    {
        Task<Category?> GetBySlugAsync(string slug);
    }
}
