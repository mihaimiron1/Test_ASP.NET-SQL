SELECT
    ps.PollingStationId,

    -- LOCALITATEA SECȚIEI (sat / oraș)
    ps.RegionId AS LocalityId,

    -- PĂRINTELE LOCALITĂȚII (localitate-mamă SAU raion)
    r.ParentId AS ParentLocalityId,

    -- RAIONUL ADEVĂRAT (părintele părintelui)
    (SELECT ParentId FROM Region rr WHERE rr.RegionId = r.ParentId) AS RaionId,

    ------------------------------------
    -- DATE PARTID / CANDIDAT
    ------------------------------------
    er.ElectionCompetitorId AS PartyId,
    ec.Code AS PartyCode,
    ec.NameRo AS PartyName,
    ec.ColorLogo AS ColorLogo,

    er.ElectionCompetitorMemberId AS CandidateMemberId,
    CONCAT(
        COALESCE(ecm.LastNameRo, ''), ' ',
        COALESCE(ecm.NameRo, '')
    ) AS CandidateFullName,

    er.BallotCount AS Votes,

    ------------------------------------
    -- BULETINE
    ------------------------------------
    bp.BallotsReceived AS TotalBallots,
    bp.BallotsValidVotes AS ValidBallots,
    bp.BallotsSpoiled AS SpoiledBallots,
    bp.BallotsUnusedSpoiled AS UnusedSpoiledBallots,
    bp.BallotsCasted AS CastedBallots,
    bp.RegisteredVoters AS RegisteredVoters,
    bp.DifferenceIssuedCasted AS Difference

FROM ElectionResult er
JOIN BallotPaper bp
    ON bp.BallotPaperId = er.BallotPaperId
JOIN PollingStation ps
    ON ps.PollingStationId = bp.PollingStationId
JOIN Region r
    ON r.RegionId = ps.RegionId
JOIN ElectionRound erd
    ON erd.ElectionRoundId = er.ElectionRoundId
JOIN ElectionCompetitor ec
    ON ec.ElectionCompetitorId = er.ElectionCompetitorId
LEFT JOIN ElectionCompetitorMember ecm
    ON ecm.ElectionCompetitorMemberId = er.ElectionCompetitorMemberId

WHERE erd.ElectionId = %s
ORDER BY ps.PollingStationId, ec.BallotOrder;