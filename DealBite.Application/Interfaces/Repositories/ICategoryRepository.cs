using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface ICategoryRepository:IGenericRepository<Category>
    {
        Task<Category?> GetBySlugAsync(string slug);
    }
}
