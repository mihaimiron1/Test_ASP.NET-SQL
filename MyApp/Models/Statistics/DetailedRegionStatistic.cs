// Models/Statistics/DetailedRegionStatistic.cs
namespace MyApp.Models.Statistics
{
    public class DetailedRegionStatistic
    {
        // Info despre regiunea/localitatea selectată
        public int RegionId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public int RegionTypeId { get; set; } = 0;

        // Statistici generale
        public int TotalVoters { get; set; }
        public int VotedCount { get; set; }
        
        // Calculated property - calculate voting percentage on the fly
        public decimal VotingPercentage => TotalVoters > 0
            ? Math.Round((decimal)VotedCount / TotalVoters * 100, 2)
            : 0;

        // Statistici pe gen
        public List<GenderStatistic> GenderStats { get; set; } = new();

        // Statistici pe vârstă
        public List<AgeStatistic> AgeStats { get; set; } = new();

        public DateTime LastUpdated { get; set; }

        // Helper pentru a determina tipul
        public string RegionTypeName => RegionTypeId switch
        {
            2 => "Raion",
            3 => "UTA",
            4 => "Municipiu",
            _ => "Regiune"
        };
    }
}