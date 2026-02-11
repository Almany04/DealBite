using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit {  get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string? ImageUrl { get; set; } 
        public ICollection<ProductPriceDto> Prices { get; set; }= new List<ProductPriceDto>();
    }
}
