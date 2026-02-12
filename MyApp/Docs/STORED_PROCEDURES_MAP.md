# ?? Stored Procedures Usage - C# Repository Map

**IMPORTANT:** C# **NU** con?ine SQL! Toate query-urile sunt în **Microsoft SQL Server** sub form? de stored procedures.

---

## ? Stored Procedures Apelate din C#

### 1?? **sp_GetRaionVotingStatsForHeatMap**
?? **Loca?ie:** SQL Server Database  
?? **Apelat din:** `StatisticsRepository.GetAllRegionsStatisticsAsync()`  
?? **Parametri:** Niciunul  
?? **Output:**
```
RegionId    : int
Name        : string
NameRu      : string (nullable)
TotalVoters : int
LastUpdated : DateTime
```

---

### 2?? **sp_GetRaionGender**
?? **Loca?ie:** SQL Server Database  
?? **Apelat din:** `StatisticsRepository.GetRegionDetailedStatisticsAsync(regionId)`  
?? **Parametri:** `@RegionId` (BigInt)  
?? **Output:**
```
Gender     : int (1=B?rba?i, 2=Femei)
VoterCount : int
VotedCount : int
```

---

### 3?? **sp_GetRaionAgeCategory**
?? **Loca?ie:** SQL Server Database  
?? **Apelat din:** `StatisticsRepository.GetRegionDetailedStatisticsAsync(regionId)`  
?? **Parametri:** `@RegionId` (BigInt)  
?? **Output:**
```
AgeCategoryId : int (1-7)
VoterCount    : int
```

---

### 4?? **sp_GetLocalitateGender**
?? **Loca?ie:** SQL Server Database  
?? **Apelat din:** `StatisticsRepository.GetLocalityDetailedStatisticsAsync(regionId)`  
?? **Parametri:** `@RegionId` (BigInt)  
?? **Output:**
```
Gender     : int (1=B?rba?i, 2=Femei)
VoterCount : int
```

---

### 5?? **sp_GetLocalitateAgeCategory**
?? **Loca?ie:** SQL Server Database  
?? **Apelat din:** `StatisticsRepository.GetLocalityDetailedStatisticsAsync(regionId)`  
?? **Parametri:** `@RegionId` (BigInt)  
?? **Output:**
```
AgeCategoryId : int (1-7)
VoterCount    : int
```

---

### 6?? **sp_GetLocalitiesByRaion**
?? **Loca?ie:** SQL Server Database  
?? **Apelat din:** `StatisticsRepository.GetLocalitiesByRegionAsync(regionId)`  
?? **Parametri:** `@RegionId` (BigInt)  
?? **Output:**
```
RegionId       : int
LocalitateName : string
RegionTypeId   : int
```

---

## ?? Cum Func?ioneaz?

### **C# Repository Face:**
1. ? Apeleaz? stored procedure din SQL Server
2. ? Prime?te date (Dapper mapeaz? automat)
3. ? Transform? valorile (ex: 1 ? "Barbati")
4. ? Calculeaz? procente
5. ? Returneaz? rezultatul

### **C# Repository NU Face:**
- ? SQL queries inline
- ? String concatenation pentru SQL
- ? Table joins în cod
- ? WHERE clauses în cod

---

## ?? Exemplu de Apel

```csharp
// ? CORECT - Apeleaz? SP din SQL Server
var result = await connection.QueryAsync<RegionVotingStatistic>(
    "sp_GetRaionVotingStatsForHeatMap",
    commandType: CommandType.StoredProcedure,
    commandTimeout: 120);

// ? GRE?IT - SQL inline în C#
var sql = "SELECT * FROM Region WHERE...";
var result = await connection.QueryAsync(sql);
```

---

## ??? Unde Sunt Stored Procedures?

**Toate** stored procedures sunt în **Microsoft SQL Server**:
- Database: `SAISE.ElectionDay20231105`
- Schema: `dbo`
- Type: Stored Procedures

Pentru a vedea sau modifica SP:
1. Deschide **SQL Server Management Studio**
2. Navigheaz? la: `Databases ? SAISE.ElectionDay20231105 ? Programmability ? Stored Procedures`
3. Click dreapta pe SP ? `Modify`

---

## ?? Dac? Trebuie Modificat SQL

**NU modifica în C#!** Modific? în SQL Server:

1. Deschide SSMS
2. G?se?te stored procedure-ul
3. Modific? logica SQL
4. `Execute` pentru a salva
5. C# va folosi automat versiunea nou?

---

## ?? Testare

### Test Stored Procedures în SQL Server:
```sql
-- Test 1: Lista raioane
EXEC sp_GetRaionVotingStatsForHeatMap;

-- Test 2: Gender pentru Chi?in?u (RegionId=2)
EXEC sp_GetRaionGender @RegionId = 2;

-- Test 3: Vârst? pentru Chi?in?u
EXEC sp_GetRaionAgeCategory @RegionId = 2;

-- Test 4: Localit??i în raion
EXEC sp_GetLocalitiesByRaion @RegionId = 2;
```

### Test C# Repository:
```csharp
// În aplica?ie, verific? log-urile:
_logger.LogInformation("Calling sp_GetRaionVotingStatsForHeatMap...");
_logger.LogInformation("sp_GetRaionVotingStatsForHeatMap returned {Count} rows", result.Count());
```

---

## ?? Structura Proiectului

```
MyApp/
??? Repositories/
?   ??? StatisticsRepository.cs  ? Apeleaz? DOAR SP-uri
??? Controllers/
?   ??? StatisticsController.cs  ? Folose?te Repository
??? Models/
?   ??? Statistics/              ? Modele C# pentru date
??? Docs/
    ??? STORED_PROCEDURES_MAP.md ? Acest document
```

**SQL Server:**
```
SAISE.ElectionDay20231105
??? Programmability
    ??? Stored Procedures
        ??? sp_GetRaionVotingStatsForHeatMap
        ??? sp_GetRaionGender
        ??? sp_GetRaionAgeCategory
        ??? sp_GetLocalitateGender
        ??? sp_GetLocalitateAgeCategory
        ??? sp_GetLocalitiesByRaion
```

---

## ? Checklist

- [x] C# con?ine **ZERO** SQL inline
- [x] Toate query-urile sunt în SQL Server
- [x] C# apeleaz? doar stored procedures
- [x] Modific?rile SQL se fac în SSMS, nu în C#
- [x] Repository folose?te Dapper pentru mapare automat?
- [x] Logging clar pentru debugging

---

**TL;DR:** C# = Doar apeluri la SP. SQL = În SQL Server. Modific?ri SQL = În SSMS, nu în C#!
