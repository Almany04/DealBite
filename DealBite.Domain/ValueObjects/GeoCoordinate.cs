using System;

namespace DealBite.Domain.ValueObjects
{
    public readonly record struct GeoCoordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "A szélességnek -90 és 90 között kell lennie.");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "A hosszúságnak -180 és 180 között kell lennie.");

            Latitude = latitude;
            Longitude = longitude;
        }

        public double GetDistanceTo(GeoCoordinate other)
        {
            var d1 = Latitude * (Math.PI / 180.0);
            var n1 = Longitude * (Math.PI / 180.0);
            var d2 = other.Latitude * (Math.PI / 180.0);
            var n2 = other.Longitude * (Math.PI / 180.0);

            var d3 = d2 - d1;
            var n3 = n2 - n1;

            var a = Math.Pow(Math.Sin(d3 / 2.0), 2.0) +
                    Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(n3 / 2.0), 2.0);

            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            return 6371 * c;
        }

        public override string ToString() => $"{Latitude}, {Longitude}";
    }
}