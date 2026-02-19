using DealBite.Domain.Enums;
using DealBite.Domain.ValueObjects;

namespace DealBite.Domain.Services
{
    public static class ReferencePriceCalculator
    {
        private static decimal Median(List<decimal> list)
        {
            var count = list.Count();
            
            if (count % 2 == 0)
            {
                return (list[count / 2 - 1] + list[count / 2]) / 2m;
            }
            else
            {
                return list[count / 2];
            }
        }
        private static List<decimal> ApplyIqrFilter(IReadOnlyList<decimal> prices)
        {
            var sortedList = prices.Order().ToList();
            if (sortedList.Count() < 5)
            {
                return sortedList;
            }

            var half = sortedList.Count / 2;

            var lowerHalf = sortedList.Take(half).ToList();
            var upperHalf = sortedList.Skip(sortedList.Count-half).ToList();

            var q1 = Median(lowerHalf);
            var q3 = Median(upperHalf);
            var iqr = q3 - q1;

            var lowerBound = q1 - (1.5m * iqr);
            var upperBound = q3 + (1.5m * iqr);

            return sortedList.Where(p => p >= lowerBound && p <= upperBound).ToList();

        }
        public static ReferencePrice Calculate(IReadOnlyList<decimal> prices)
        {
            if (prices == null)
            {
                throw new ArgumentNullException(nameof(prices));
            }
            if (prices.Count.Equals(0))
            {
                return new ReferencePrice(Money.Zero, DateTimeOffset.UtcNow, 0);
            }

            var filteredPrices=ApplyIqrFilter(prices);

            decimal finalMedian = Median(filteredPrices);


            return new ReferencePrice(
                MedianPrice:new Money(finalMedian),
                CalculatedAt: DateTimeOffset.UtcNow,
                SampleSize: prices.Count
                );
            

        }
    }
}
