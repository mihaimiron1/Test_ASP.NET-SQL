namespace MyApp.Models.Statistics
{
    public class StatisticsTestViewModel
    {
        public bool HeatMapOk { get; set; }
        public string? Error { get; set; }

        public string CacheInfoUrl { get; set; } = string.Empty;
        public string HeatMapJsonUrl { get; set; } = string.Empty;

        public List<RegionVotingStatistic> SampleRegions { get; set; } = new();
    }
}

