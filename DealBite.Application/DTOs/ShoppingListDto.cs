using DealBite.Domain.Entities;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class ShoppingListDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalEstimatedPrice { get; set; }
        public decimal TotalSaved { get; set; }
        public bool IsCompleted { get; set; }
        public ICollection<ShoppingListItemDto> ShoppingListItems { get; set; } = [];
    }
}
