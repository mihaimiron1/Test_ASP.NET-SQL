namespace MyApp.Models.Statistics
{
    public class RegionBasicInfo
    {
        public int RegionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RegionTypeId { get; set; }

        public string RegionTypeName => RegionTypeId switch
        {
            2 => "Raion",
            3 => "UTA",
            4 => "Municipiu",
            5 => "Localitate",
            _ => "Regiune"
        };
    }
}