using DealBite.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.ValueObjects
{
    public readonly record struct ReferencePrice(Money MedianPrice, DateTimeOffset CalculatedAt, int SampleSize)
    {
        public PriceConfidence Confidence => SampleSize >= 9 ? PriceConfidence.High : SampleSize >= 4 ? PriceConfidence.Medium : PriceConfidence.Low;
    }
}
