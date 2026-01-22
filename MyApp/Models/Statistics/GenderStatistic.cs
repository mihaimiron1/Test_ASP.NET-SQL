// Models/Statistics/GenderStatistic.cs
namespace MyApp.Models.Statistics
{
    public class GenderStatistic
    {
        // Date din SQL (vine deja ca string din query-ul tău)
        public string Gender { get; set; } = string.Empty; // "Barbati", "Femei", "Necunoscut"
        public int VoterCount { get; set; }
        public int VotedCount { get; set; } // ← ADĂUGAT (din query-ul tău)

        // Pentru grafice - culori
        public string Color => Gender switch
        {
            "Barbati" => "#3498db",
            "Femei" => "#e74c3c",
            _ => "#95a5a6"
        };

        // Procentul se calculează în repository/controller
        public decimal Percentage { get; set; }
    }
}