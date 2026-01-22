// Models/Statistics/AgeStatistic.cs
namespace MyApp.Models.Statistics
{
    public class AgeStatistic
    {
        // Date din SQL
        public int AgeCategoryId { get; set; }
        public int VoterCount { get; set; }

        // Calculated properties
        public string AgeCategoryName => AgeCategoryId switch
        {
            1 => "18-25 ani",
            2 => "26-35 ani",
            3 => "36-45 ani",
            4 => "46-55 ani",
            5 => "56-65 ani",
            6 => "66-75 ani",
            7 => "76+ ani",
            _ => "Necunoscut"
        };

        // Culori pentru bar chart
        public string Color => AgeCategoryId switch
        {
            1 => "#e3f2fd",
            2 => "#bbdefb",
            3 => "#90caf9",
            4 => "#64b5f6",
            5 => "#42a5f5",
            6 => "#2196f3",
            7 => "#1976d2",
            _ => "#95a5a6"
        };

        public decimal Percentage { get; set; }
    }
}