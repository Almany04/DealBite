using DealBite.Domain.Common;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class ShoppingList:BaseEntity
    {
        public required string Name { get; set; }
        public Money TotalEstimatedPrice { get; set; }
        public Money TotalSaved {  get; set; }
        public bool IsCompleted { get; set; }
        public Guid UserId { get; set; }
        public AppUser? User { get; set; }
        public ICollection<ShoppingListItem> ShoppingListItems { get; set; } = [];

    }
}
