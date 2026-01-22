namespace MyApp.Models
{
    public class RegionStatistic
    {
        public int AssignedVoterId { get; set; }
        public int AssignedVoterStatus { get; set; }
        public int Gender { get; set; }
        public int AgeCategoryId { get; set; }
        public int PollingStationId { get; set;}    
        public int RegionId { get; set; }
        public int ParentRegionId { get; set; }
        public required DateTime CreationDate { get; set; }



    }
}
