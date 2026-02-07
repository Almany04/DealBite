using DealBite.Domain.Common;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Entities
{
    public class ShoppingListItem:BaseEntity
    {
        public required string ProductName { get; set; }
        public double Quantity { get; set; }
        public bool IsChecked { get; set; }
        public Money EstimatedPrice { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
