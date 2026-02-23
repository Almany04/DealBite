using DealBite.Domain.Entities;
using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.Services
{
    public static class PriceEvaluator
    {
        public static (decimal DeviationPercent, PriceEvaluation priceEvaluation) PriceCalculator(decimal unitPrice, decimal? referencePrice)
        {
            if (referencePrice == null || referencePrice==0) {
                return (0, PriceEvaluation.Average);
            }

            var deviationPrice = (referencePrice - unitPrice) / referencePrice * 100;

            var result = deviationPrice switch
            {
                > 10 => PriceEvaluation.Excellent,
                >= -10 and <=10 => PriceEvaluation.Average,
                _ => PriceEvaluation.Expensive
            };

            return (deviationPrice.Value, result);
        }
    }
}
