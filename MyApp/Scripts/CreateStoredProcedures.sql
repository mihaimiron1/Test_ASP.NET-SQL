-- =========================================================
-- STORED PROCEDURES FOR STATISTICS
-- =========================================================
-- This script creates 6 stored procedures for statistics queries
-- Run this script in your SQL Server database
-- IMPORTANT: Remove or comment out the USE statement if running in Azure SQL
-- =========================================================

USE [SAISE.ElectionDay20231105]
GO

-- =========================================================
-- 1. sp_GetRaionVotingStatsForHeatMap (MODIFICAT)
-- Output: RegionId, Name, NameRu, TotalVoters, LastUpdated
-- Filtru: doar raioane/UTA/municipii care au cel putin 1 vot (AssignedVoterStatus >= 5000)
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetRaionVotingStatsForHeatMap]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetRaionVotingStatsForHeatMap]
GO

CREATE PROCEDURE [dbo].[sp_GetRaionVotingStatsForHeatMap]
AS
BEGIN
    SET NOCOUNT ON;

    -- Index pe AVS
    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_AssignedVoterStatistics_RegionId_Status'
          AND object_id = OBJECT_ID('dbo.AssignedVoterStatistics')
    )
    BEGIN
        CREATE INDEX IX_AssignedVoterStatistics_RegionId_Status
        ON dbo.AssignedVoterStatistics(RegionId)
        INCLUDE (AssignedVoterStatus);
    END;

    -- Index pe Region (include si NameRu)
    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_Region_RegionId_ParentId_Inc'
          AND object_id = OBJECT_ID('dbo.Region')
    )
    BEGIN
        CREATE INDEX IX_Region_RegionId_ParentId_Inc
        ON dbo.Region(RegionId, ParentId)
        INCLUDE (RegionTypeId, Name, NameRu);
    END;

    ;WITH DistinctRegions AS (
        SELECT DISTINCT avs.RegionId
        FROM dbo.AssignedVoterStatistics avs
        WHERE avs.RegionId IS NOT NULL
    ),
    Tree AS (
        -- nivel 0
        SELECT
            dr.RegionId AS StartRegionId,
            r.RegionId,
            r.ParentId,
            r.RegionTypeId,
            r.Name,
            r.NameRu,
            0 AS Lvl
        FROM DistinctRegions dr
        JOIN dbo.Region r
            ON r.RegionId = dr.RegionId

        UNION ALL

        -- urcam: STOP daca ajungem la 2 sau 3 (preferate)
        SELECT
            t.StartRegionId,
            p.RegionId,
            p.ParentId,
            p.RegionTypeId,
            p.Name,
            p.NameRu,
            t.Lvl + 1
        FROM Tree t
        JOIN dbo.Region p
            ON p.RegionId = t.ParentId
        WHERE t.ParentId IS NOT NULL
          AND t.RegionTypeId NOT IN (2,3)
    ),
    Pick AS (
        SELECT
            StartRegionId,
            RegionId     AS PickRegionId,
            Name         AS PickName,
            NameRu       AS PickNameRu,
            ROW_NUMBER() OVER (
                PARTITION BY StartRegionId
                ORDER BY
                    CASE
                        WHEN RegionTypeId IN (2,3) THEN 0
                        WHEN RegionTypeId = 4 THEN 1
                        ELSE 2
                    END,
                    Lvl ASC
            ) AS rn
        FROM Tree
        WHERE RegionTypeId IN (2,3,4)
    ),
    MapFinal AS (
        SELECT StartRegionId, PickRegionId, PickName, PickNameRu
        FROM Pick
        WHERE rn = 1
    )
    SELECT
        m.PickRegionId AS RegionId,
        m.PickName     AS [Name],
        m.PickNameRu   AS NameRu,
        COUNT(*) AS TotalVoters,
        GETDATE() AS LastUpdated
    FROM dbo.AssignedVoterStatistics avs
    JOIN MapFinal m
        ON m.StartRegionId = avs.RegionId
    WHERE avs.RegionId IS NOT NULL
    GROUP BY
        m.PickRegionId,
        m.PickName,
        m.PickNameRu
    HAVING SUM(CASE WHEN avs.AssignedVoterStatus >= 5000 THEN 1 ELSE 0 END) > 0
    ORDER BY
        m.PickName
    OPTION (MAXRECURSION 200);
END
GO

-- =========================================================
-- 2. sp_GetRaionGender
-- Returns gender statistics for a specific raion/UTA/municipiu
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetRaionGender]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetRaionGender]
GO

CREATE PROCEDURE [dbo].[sp_GetRaionGender]
    @RegionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validare: permitem doar 2/3/4
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Region r
        WHERE r.RegionId = @RegionId
          AND r.RegionTypeId IN (2,3,4)
    )
    BEGIN
        RAISERROR('RegionId trebuie sa fie RegionTypeId IN (2,3,4): raion / UTA / municipiu.', 16, 1);
        RETURN;
    END;

    IF OBJECT_ID('tempdb..#Down') IS NOT NULL DROP TABLE #Down;

    ;WITH Down AS (
        SELECT CAST(@RegionId AS BIGINT) AS RegionId
        UNION ALL
        SELECT CAST(c.RegionId AS BIGINT) AS RegionId
        FROM Down d
        JOIN dbo.Region c
            ON c.ParentId = d.RegionId
    )
    SELECT RegionId
    INTO #Down
    FROM Down
    OPTION (MAXRECURSION 500);

    CREATE UNIQUE CLUSTERED INDEX IX__Down_RegionId ON #Down(RegionId);

    SELECT
        avs.Gender,
        COUNT(*) AS VoterCount,
        SUM(CASE WHEN avs.AssignedVoterStatus >= 5000 THEN 1 ELSE 0 END) AS VotedCount
    FROM #Down d
    JOIN dbo.AssignedVoterStatistics avs
        ON avs.RegionId = d.RegionId
    GROUP BY
        avs.Gender
    ORDER BY avs.Gender;

    DROP TABLE #Down;
END
GO

-- =========================================================
-- 3. sp_GetRaionAgeCategory
-- Returns age category statistics for a specific raion/UTA
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetRaionAgeCategory]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetRaionAgeCategory]
GO

CREATE PROCEDURE [dbo].[sp_GetRaionAgeCategory]
    @RegionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validare: acceptam doar nivel 2 sau 4
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Region r
        WHERE r.RegionId = @RegionId
          AND r.RegionTypeId IN (2,4)
    )
    BEGIN
        RAISERROR('RegionId trebuie sa fie RegionTypeId IN (2,4) (raion / UTA).', 16, 1);
        RETURN;
    END;

    ;WITH Down AS (
        -- luam tot subarborele (toate localitatile/sub-regiunile din raion/UTA) 
        SELECT r.RegionId
        FROM dbo.Region r
        WHERE r.RegionId = @RegionId

        UNION ALL

        SELECT c.RegionId
        FROM Down d
        JOIN dbo.Region c
            ON c.ParentId = d.RegionId
    )
    SELECT
        avs.AgeCategoryId,
        COUNT(*) AS VoterCount
    FROM Down d
    JOIN dbo.AssignedVoterStatistics avs
        ON avs.RegionId = d.RegionId
    WHERE avs.AssignedVoterStatus >= 5000
    GROUP BY
        avs.AgeCategoryId
    ORDER BY
        avs.AgeCategoryId
    OPTION (MAXRECURSION 500);
END
GO

-- =========================================================
-- 4. sp_GetLocalitateGender
-- Returns gender statistics for a specific locality
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetLocalitateGender]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetLocalitateGender]
GO

CREATE PROCEDURE [dbo].[sp_GetLocalitateGender]
    @RegionId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Gender,
        COUNT(*) AS VoterCount
    FROM AssignedVoterStatistics
    WHERE AssignedVoterStatus >= 5000
        AND (@RegionId IS NULL OR RegionId = @RegionId)
    GROUP BY Gender
    ORDER BY Gender;
END
GO

-- =========================================================
-- 5. sp_GetLocalitateAgeCategory
-- Returns age category statistics for a specific locality
-- FIXED: Changed COUNT() to COUNT(*)
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetLocalitateAgeCategory]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetLocalitateAgeCategory]
GO

CREATE PROCEDURE [dbo].[sp_GetLocalitateAgeCategory]
    @RegionId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        AgeCategoryId,
        COUNT(*) AS VoterCount  -- FIXED: was COUNT()
    FROM AssignedVoterStatistics
    WHERE AssignedVoterStatus = 5000
        AND (@RegionId IS NULL OR RegionId = @RegionId)
    GROUP BY AgeCategoryId
    ORDER BY AgeCategoryId;
END
GO

-- =========================================================
-- 6. sp_GetLocalitiesByRaion
-- Returns list of localities within a raion/UTA/municipiu
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetLocalitiesByRaion]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetLocalitiesByRaion]
GO

CREATE PROCEDURE [dbo].[sp_GetLocalitiesByRaion]
    @RegionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validare
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Region
        WHERE RegionId = @RegionId
          AND RegionTypeId IN (2,3,4)
    )
    BEGIN
        RAISERROR('RegionId trebuie sa fie RegionTypeId IN (2,3,4): raion / UTA / municipiu.', 16, 1);
        RETURN;
    END;

    IF OBJECT_ID('tempdb..#Down') IS NOT NULL DROP TABLE #Down;

    ;WITH Down AS (
        SELECT RegionId
        FROM dbo.Region
        WHERE RegionId = @RegionId

        UNION ALL

        SELECT c.RegionId
        FROM Down d
        JOIN dbo.Region c
            ON c.ParentId = d.RegionId
    )
    SELECT RegionId
    INTO #Down
    FROM Down
    OPTION (MAXRECURSION 500);

    -- Index for performance
    CREATE UNIQUE CLUSTERED INDEX IX__Down_RegionId ON #Down(RegionId);

    -- LOCALITATI care exista in AssignedVoterStatistics
    SELECT DISTINCT
        r.RegionId,
        r.Name AS LocalitateName,
        r.RegionTypeId
    FROM #Down d
    JOIN dbo.AssignedVoterStatistics avs
        ON avs.RegionId = d.RegionId
    JOIN dbo.Region r
        ON r.RegionId = d.RegionId
    WHERE r.RegionId <> @RegionId           -- excludem raionul insusi
      AND r.RegionTypeId IN (2,3,4,6,7,8,9)
    ORDER BY
        r.Name;

    DROP TABLE #Down;
END
GO

-- =========================================================
-- 7. sp_GetRegionsInDatabase
-- FAST:
--  - ia RegionId distinct din AssignedVoterStatistics
--  - urca pana la primul nod (2/3/4) (stop devreme)
--  - returneaza lista distincta (RegionId, RegionName)
-- NOTE:
--  The SQL logic below is kept the same as provided (wrapped in a stored procedure).
-- =========================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetRegionsInDatabase]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetRegionsInDatabase]
GO

CREATE PROCEDURE [dbo].[sp_GetRegionsInDatabase]
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================================================
       FAST:
       - ia RegionId distinct din AssignedVoterStatistics
       - urca pana la primul nod (2/3/4) (stop devreme)
       - returneaza lista distincta (RegionId, RegionName)
       ========================================================= */

    -- Indexuri (daca nu exista) - ajuta mult
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = 'IX_AVS_RegionId'
          AND object_id = OBJECT_ID('dbo.AssignedVoterStatistics')
    )
    BEGIN
        CREATE INDEX IX_AVS_RegionId ON dbo.AssignedVoterStatistics(RegionId);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = 'IX_Region_RegionId_ParentId_Inc'
          AND object_id = OBJECT_ID('dbo.Region')
    )
    BEGIN
        CREATE INDEX IX_Region_RegionId_ParentId_Inc
        ON dbo.Region(RegionId, ParentId)
        INCLUDE (RegionTypeId, Name);
    END;

    IF OBJECT_ID('tempdb..#Top') IS NOT NULL DROP TABLE #Top;

    ;WITH DistinctRegions AS (
        SELECT DISTINCT CAST(avs.RegionId AS BIGINT) AS StartRegionId
        FROM dbo.AssignedVoterStatistics avs
        WHERE avs.RegionId IS NOT NULL
    ),
    Walk AS (
        -- start
        SELECT
            dr.StartRegionId,
            r.RegionId,
            r.ParentId,
            r.RegionTypeId,
            r.Name,
            0 AS Lvl
        FROM DistinctRegions dr
        JOIN dbo.Region r
            ON r.RegionId = dr.StartRegionId

        UNION ALL

        -- stop devreme: nu mai urcam daca nodul curent e 2/3/4
        SELECT
            w.StartRegionId,
            p.RegionId,
            p.ParentId,
            p.RegionTypeId,
            p.Name,
            w.Lvl + 1
        FROM Walk w
        JOIN dbo.Region p
            ON p.RegionId = w.ParentId
        WHERE w.ParentId IS NOT NULL
          AND w.RegionTypeId NOT IN (2,3,4)
    ),
    Pick AS (
        -- pentru fiecare StartRegionId luam primul 2/3/4 gasit (cel mai apropiat in sus)
        SELECT
            StartRegionId,
            RegionId AS TopRegionId,
            Name     AS TopRegionName,
            ROW_NUMBER() OVER (PARTITION BY StartRegionId ORDER BY Lvl ASC) AS rn
        FROM Walk
        WHERE RegionTypeId IN (2,3,4)
    )
    SELECT
        TopRegionId,
        TopRegionName
    INTO #Top
    FROM Pick
    WHERE rn = 1
    OPTION (MAXRECURSION 200);

    -- index pe temp table pentru DISTINCT rapid
    CREATE INDEX IX__Top_TopRegionId ON #Top(TopRegionId);

    -- OUTPUT final (fara RegionTypeId)
    SELECT DISTINCT
        t.TopRegionId AS RegionId,
        t.TopRegionName AS RegionName
    FROM #Top t
    ORDER BY t.TopRegionName;
END
GO

PRINT 'All stored procedures created successfully!'
GO
