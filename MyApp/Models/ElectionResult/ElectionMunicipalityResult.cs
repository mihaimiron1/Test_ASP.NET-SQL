namespace MyApp.Models.ElectionResult
{
    public class ElectionMunicipalityResult
    {
        public int MunicipiuId { get; set; }
        public string MunicipiuName { get; set; }
        public int PartyId { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public string ColorLogo { get; set; }
        public int CandidateMemberId { get; set; }
        public string CandidateFullName { get; set; }
        public int Votes { get; set; }
        public int TotalBallots { get; set; }
        public int ValidBallots { get; set; }
        public int SpoiledBallots { get; set; }
        public int UnusedSpoiledBallots { get; set; }
        public int CastedBallots { get; set; }
        public int RegisteredVoters { get; set; }
        public int Difference { get; set; }
    }
}