using System;

namespace Client
{
    public class Rate
    {
        public int CurrencyId { get; set; }
        public string PriceCurrency { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}
