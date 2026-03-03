namespace MyApp.Models.ElectionResult;

public class ElectionResultByPollingStationDto
{
    public long PollingStationId { get; set; }
    public long LocalityId { get; set; }
    public long ParentLocalityId { get; set; }
    public long RaionId { get; set; }
    public long PartyId { get; set; }
    public string PartyCode { get; set; }
    public string PartyName { get; set; }
    public byte[] ColorLogo { get; set; }
    public long? CandidateMemberId { get; set; }
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