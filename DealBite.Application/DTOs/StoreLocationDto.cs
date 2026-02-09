using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.DTOs
{
    public class StoreLocationDto
    {
        public string Address { get; set; } =string.Empty;
        public string City { get; set; } =string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceInMeters { get; set; }


    }
}
