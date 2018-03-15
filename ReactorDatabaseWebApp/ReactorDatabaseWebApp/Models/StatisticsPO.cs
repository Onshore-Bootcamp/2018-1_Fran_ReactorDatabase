namespace ReactorDatabaseWebApp.Models
{
    public class StatisticsPO
    {
        public int ReactorCount { get; set; }

        public int ThermalCount { get; set; }

        public int FastCount { get; set; }

        public decimal PercentThermal { get; set; }

        public decimal PercentFast { get; set; }

        public int GenIcount { get; set; }

        public int GenIIcount { get; set; }

        public int GenIIIcount { get; set; }

        public int GenIVcount { get; set; }

        public int GenVcount { get; set; }

        public int NoGenCount { get; set; }

        public decimal PercentGenI { get; set; }

        public decimal PercentGenII { get; set; }

        public decimal PercentGenIII { get; set; }

        public decimal PercentGenIV { get; set; }

        public decimal PercentGenV { get; set; }

        public decimal PercentNoGen { get; set; }
    }
}