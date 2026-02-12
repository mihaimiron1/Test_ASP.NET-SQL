# Diagnostic Stored Procedures pentru HeatMap

## Problema Curent?
**Eroare**: `Execution Timeout Expired` la înc?rcarea paginii HeatMap

## Stored Procedures Folosite

### 1. `sp_GetRaionVotingStatsForHeatMap`
**Scop**: Returneaz? lista de raioane cu num?rul total de aleg?tori  
**Apelat de**: `StatisticsRepository.GetAllRegionsStatisticsAsync()`  
**Frecven??**: La fiecare înc?rcare a paginii (cu cache de 10 min)

**Ce ar trebui s? returneze**:
```sql
-- Doar RegionId, Name, NameRu, TotalVoters
SELECT 
    r.RegionId,
    r.Name,
    r.NameRu,
    COUNT(*) AS TotalVoters,
    GETDATE() AS LastUpdated
FROM Region r
INNER JOIN AssignedVoterStatistics avs ON r.RegionId = avs.RegionId
WHERE r.RegionTypeId IN (2, 3, 4) -- Doar raioane, UTA, municipii
GROUP BY r.RegionId, r.Name, r.NameRu
HAVING COUNT(*) > 0 -- Doar regiuni cu voturi
```

**?? IMPORTANT**: 
- NU calcule complicate
- NU JOIN-uri multiple
- NU subquery-uri
- Doar COUNT simplu

---

### 2. `sp_GetRaionGender`
**Scop**: Distribu?ie pe gen pentru UN raion  
**Apelat de**: `StatisticsRepository.GetRegionDetailedStatisticsAsync(regionId)`  
**Frecven??**: Doar când utilizatorul apas? pe raion (on-demand, cu cache)

**Ce ar trebui s? returneze**:
```sql
-- Gender (1=Barbati, 2=Femei), VotedCount
SELECT 
    Gender,
    COUNT(*) AS VotedCount
FROM AssignedVoterStatistics
WHERE RegionId = @RegionId
  AND AssignedVoterStatus >= 5000 -- Doar cei care au votat
GROUP BY Gender
```

---

### 3. `sp_GetRaionAgeCategory`
**Scop**: Distribu?ie pe categorii de vârst? pentru UN raion  
**Apelat de**: `StatisticsRepository.GetRegionDetailedStatisticsAsync(regionId)`  
**Frecven??**: Doar când utilizatorul apas? pe raion (on-demand, cu cache)

**Ce ar trebui s? returneze**:
```sql
-- AgeCategoryId, VoterCount
SELECT 
    AgeCategoryId,
    COUNT(*) AS VoterCount
FROM AssignedVoterStatistics
WHERE RegionId = @RegionId
  AND AssignedVoterStatus >= 5000 -- Doar cei care au votat
GROUP BY AgeCategoryId
ORDER BY AgeCategoryId
```

---

## Optimiz?ri Recomandate

### 1. Index-uri necesare
```sql
-- Index pentru AssignedVoterStatistics
CREATE NONCLUSTERED INDEX IX_AssignedVoterStatistics_RegionId_Status
ON AssignedVoterStatistics(RegionId, AssignedVoterStatus)
INCLUDE (Gender, AgeCategoryId);

-- Index pentru Region
CREATE NONCLUSTERED INDEX IX_Region_RegionTypeId
ON Region(RegionTypeId)
INCLUDE (RegionId, Name, NameRu);
```

### 2. Statistici tabele
```sql
-- Actualizeaz? statisticile pentru performan??
UPDATE STATISTICS AssignedVoterStatistics WITH FULLSCAN;
UPDATE STATISTICS Region WITH FULLSCAN;
```

### 3. Plan de execu?ie
```sql
-- Verific? planul de execu?ie pentru fiecare SP
SET STATISTICS TIME ON;
SET STATISTICS IO ON;

EXEC sp_GetRaionVotingStatsForHeatMap;
EXEC sp_GetRaionGender @RegionId = 2; -- Chi?in?u ca test
EXEC sp_GetRaionAgeCategory @RegionId = 2;

SET STATISTICS TIME OFF;
SET STATISTICS IO OFF;
```

---

## Testare Rapid?

### Test 1: Verific? câte rânduri returneaz?
```sql
-- Ar trebui s? fie ~36 raioane
SELECT COUNT(*) FROM (
    EXEC sp_GetRaionVotingStatsForHeatMap
) AS Results;
```

### Test 2: Verific? timpul de execu?ie
```sql
DECLARE @StartTime DATETIME2 = SYSDATETIME();
EXEC sp_GetRaionVotingStatsForHeatMap;
DECLARE @EndTime DATETIME2 = SYSDATETIME();
SELECT DATEDIFF(MILLISECOND, @StartTime, @EndTime) AS ExecutionTimeMs;

-- Ar trebui s? fie < 2000ms (2 secunde)
```

---

## Log-uri pentru Debugging

Verific? log-urile aplica?iei pentru:
```
?? Cache Service STARTED
?? Updating region list cache
? Cached X regions in Y.Ys
```

Dac? nu vezi aceste log-uri sau Y > 5 secunde, problema e la SP.

---

## Contact DBA

Dac? timeout-ul persist? dup? verificarea index-urilor:
1. Verific? fragmentarea index-urilor
2. Verific? statisticile tabelelor
3. Analizeaz? planul de execu?ie pentru scan-uri de tabel
4. Consider? partitionarea tabelului `AssignedVoterStatistics` dac? are > 10M rânduri
