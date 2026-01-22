// Models/Statistics/RegionVotingStatistic.cs
namespace MyApp.Models.Statistics
{
    public class RegionVotingStatistic
    {
        // Date din SQL (from sp_GetRaionVotingStatsForHeatMap)
        public int RegionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameRu { get; set; }
        public int TotalVoters { get; set; }
        public DateTime LastUpdated { get; set; }
        
        // Note: VotedCount and VotingPercentage removed as they're no longer in SP output
        // The SP filters to only return regions with at least 1 vote (HAVING clause)
    }
}