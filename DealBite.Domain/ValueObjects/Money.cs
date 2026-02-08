using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Domain.ValueObjects
{
    public readonly record struct Money(decimal Amount, string Currency= "HUF")
    {
        public static Money Zero => new(0, "HUF");

        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
            {
                throw new InvalidOperationException($"Nem adható össze eltérő valuta: {a.Currency} és {b.Currency}");
            }
            return new Money(a.Amount+b.Amount, a.Currency);
        }
        public static Money operator -(Money a, Money b)
        {
            if (a.Currency != b.Currency)
            {
                throw new InvalidOperationException($"Nem vonható ki eltérő valuta: {a.Currency} és {b.Currency}");
            }
            return new Money(a.Amount - b.Amount, a.Currency);
        }

        public static Money operator *(Money a, decimal multiplier)
            =>new (a.Amount * multiplier, a.Currency);
        public override string ToString() => $"{Amount:N0} {Currency}";
    }
}
