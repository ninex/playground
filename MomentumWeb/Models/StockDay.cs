using System;
namespace MomentumWeb.Models
{
    public class StockDay
    {
        public string InstrumentIdentifier { get; set; }
        public string InstrumentName { get; set; }
        public string AfrikaansInstrumentName { get; set; }
        public bool IsPortfolio { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Change { get; set; }
        public double PercentageChange { get; set; }
        public int Volume { get; set; }
        public DateTime DateStamp { get; set; }
        public string DateStampFormatted { get; set; }
        public int Movement { get; set; }
    }
}