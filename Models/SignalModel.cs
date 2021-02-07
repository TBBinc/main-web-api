using System;

namespace SiteWebApi.Models
{
    public class SignalModel
    {
        public float LatLocation {get; set;}
        public float LongLocation {get; set;}
        public int Expiry {get ; set;}
        public int Age {get;set;}
    }
}