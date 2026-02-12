# ?? Structura Procedurilor Stocate - Verificare

## Rezumat Apeluri din Cod

Aplica?ia C# apeleaz? urm?toarele stored procedures folosind **Dapper**:

### 1?? `sp_GetRaionVotingStatsForHeatMap`
**Apelat de**: `StatisticsRepository.GetAllRegionsStatisticsAsync()` (linia 32)  
**Parametri**: Niciun parametru  
**Mapare**: ? `RegionVotingStatistic` (C# class)

#### Structura EXACT? cerut? de cod:
```sql
-- Stored procedure TREBUIE s? returneze aceste coloane:
SELECT 
    RegionId      INT,           -- OBLIGATORIU
    Name          NVARCHAR(255), -- OBLIGATORIU
    NameRu        NVARCHAR(255), -- OP?IONAL (nullable)
    TotalVoters   INT,           -- OBLIGATORIU
    LastUpdated   DATETIME2      -- OBLIGATORIU
FROM ...
```

#### ? Model C# care prime?te datele:
```csharp
public class RegionVotingStatistic
{
    public int RegionId { get; set; }
    public string Name { get; set; }
    public string? NameRu { get; set; }
    public int TotalVoters { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

#### ?? NOT? IMPORTANT?:
- **NU** trebuie s? returneze `VotedCount` sau `VotingPercentage`
- Numele coloanelor trebuie s? fie **EXACT** ca în model (case-insensitive pentru Dapper)

---

### 2?? `sp_GetRaionGender`
**Apelat de**: `StatisticsRepository.GetRegionDetailedStatisticsAsync()` (linia 88)  
**Parametri**: `@RegionId INT`  
**Mapare**: ? `dynamic` ? apoi transformat în `GenderStatistic`

#### Structura EXACT? cerut? de cod:
```sql
-- Stored procedure TREBUIE s? returneze aceste coloane:
SELECT 
    Gender       INT,  -- OBLIGATORIU: 1 = B?rba?i, 2 = Femei
    VotedCount   INT   -- OBLIGATORIU: num?rul de voturi
FROM ...
WHERE RegionId = @RegionId
```

#### ? Model C# care prime?te datele:
```csharp
// Codul a?teapt?:
var result = {
    Gender = 1 sau 2 (int),
    VotedCount = num?r (int)
}

// Apoi transform? în:
public class GenderStatistic
{
    public string Gender { get; set; }    // "Barbati" sau "Femei"
    public int VoterCount { get; set; }   // = VotedCount din SP
    public int VotedCount { get; set; }   // = VotedCount din SP
    public decimal Percentage { get; set; } // calculat în C#
    public string Color { get; }          // calculat în C#
}
```

#### ?? Transformare în cod (linia 93-102):
```csharp
Gender = (int)g.Gender switch
{
    1 => "Barbati",
    2 => "Femei",
    _ => "Necunoscut"
},
VoterCount = (int)g.VotedCount,
VotedCount = (int)g.VotedCount,
```

#### ?? NOT? IMPORTANT?:
- `Gender` trebuie s? fie **INT** (nu string)
- `VotedCount` trebuie s? fie **INT**
- Percentage se calculeaz? automat în C# (nu în SP)

---

### 3?? `sp_GetRaionAgeCategory`
**Apelat de**: `StatisticsRepository.GetRegionDetailedStatisticsAsync()` (linia 117)  
**Parametri**: `@RegionId INT`  
**Mapare**: ? `AgeStatistic` (direct mapping)

#### Structura EXACT? cerut? de cod:
```sql
-- Stored procedure TREBUIE s? returneze aceste coloane:
SELECT 
    AgeCategoryId  INT,  -- OBLIGATORIU: 1-7 (categorii de vârst?)
    VoterCount     INT   -- OBLIGATORIU: num?rul de voturi
FROM ...
WHERE RegionId = @RegionId
ORDER BY AgeCategoryId
```

#### ? Model C# care prime?te datele:
```csharp
public class AgeStatistic
{
    public int AgeCategoryId { get; set; }   // 1-7
    public int VoterCount { get; set; }      // num?r voturi
    
    // Calculated properties (automate în C#):
    public string AgeCategoryName { get; }   // "18-25 ani", etc.
    public string Color { get; }             // culori pentru grafic
    public decimal Percentage { get; set; }  // calculat în C#
}
```

#### ?? Mapare AgeCategoryId ? Nume (automat? în C#):
```
1 ? "18-25 ani"
2 ? "26-35 ani"
3 ? "36-45 ani"
4 ? "46-55 ani"
5 ? "56-65 ani"
6 ? "66-75 ani"
7 ? "76+ ani"
```

#### ?? NOT? IMPORTANT?:
- `AgeCategoryId` trebuie s? fie **1-7** (int)
- `VoterCount` trebuie s? fie **INT**
- **NU** trebuie s? returnezi `AgeCategoryName` sau `Color` - se genereaz? automat
- Percentage se calculeaz? automat în C#

---

### 4?? `sp_GetLocalitateGender`
**Apelat de**: `StatisticsRepository.GetLocalityDetailedStatisticsAsync()`  
**Parametri**: `@RegionId INT`  
**Mapare**: ? `dynamic` ? apoi transformat în `GenderStatistic`

#### Structura identic? cu `sp_GetRaionGender`:
```sql
SELECT 
    Gender       INT,  -- 1 = B?rba?i, 2 = Femei
    VoterCount   INT   -- num?rul de voturi (deja filtrat >= 5000)
FROM ...
WHERE RegionId = @RegionId
```

---

### 5?? `sp_GetLocalitateAgeCategory`
**Apelat de**: `StatisticsRepository.GetLocalityDetailedStatisticsAsync()`  
**Parametri**: `@RegionId INT`  
**Mapare**: ? `AgeStatistic` (direct mapping)

#### Structura identic? cu `sp_GetRaionAgeCategory`:
```sql
SELECT 
    AgeCategoryId  INT,
    VoterCount     INT
FROM ...
WHERE RegionId = @RegionId
ORDER BY AgeCategoryId
```

---

### 6?? `sp_GetLocalitiesByRaion`
**Apelat de**: `StatisticsRepository.GetLocalitiesByRegionAsync()`  
**Parametri**: `@RegionId INT`  
**Mapare**: ? `dynamic` ? apoi transformat în `RegionBasicInfo`

#### Structura EXACT? cerut? de cod:
```sql
SELECT 
    RegionId        INT,           -- OBLIGATORIU
    LocalitateName  NVARCHAR(255), -- OBLIGATORIU (NU "Name")
    RegionTypeId    INT            -- OBLIGATORIU
FROM ...
WHERE ParentRegionId = @RegionId
```

#### ?? NOT? CRITIC?:
- Coloana trebuie s? se numeasc? **`LocalitateName`** (nu `Name`)
- Codul face mapare explicit?: `Name = r.LocalitateName`

---

## ? Checklist pentru Verificare

Dup? ce modifici stored procedures în SQL Server, verific?:

### 1. Nume coloane EXACT ca mai sus
```sql
-- ? CORECT:
SELECT RegionId, Name, NameRu, TotalVoters, LastUpdated

-- ? GRE?IT:
SELECT RegionId, RegionName, NameRu, TotalVoters, LastUpdated
--           ^^^ - va e?ua, trebuie "Name" nu "RegionName"
```

### 2. Tipuri de date compatibile
```sql
RegionId      ? INT
Name          ? NVARCHAR (orice lungime)
Gender        ? INT (1 sau 2, NU string)
AgeCategoryId ? INT (1-7)
VoterCount    ? INT
TotalVoters   ? INT
LastUpdated   ? DATETIME sau DATETIME2
```

### 3. Parametri cu nume exact
```sql
-- ? CORECT:
CREATE PROCEDURE sp_GetRaionGender
    @RegionId INT  -- exact acest nume
AS

-- ? GRE?IT:
CREATE PROCEDURE sp_GetRaionGender
    @regionId INT  -- C# trimite @RegionId (capital R)
```

### 4. Testare rapid? în SQL
```sql
-- Test pentru fiecare SP:
EXEC sp_GetRaionVotingStatsForHeatMap;
EXEC sp_GetRaionGender @RegionId = 2;
EXEC sp_GetRaionAgeCategory @RegionId = 2;
EXEC sp_GetLocalitiesByRaion @RegionId = 2;

-- Verific?:
-- 1. Returneaz? rânduri?
-- 2. Coloanele au numele corect?
-- 3. Valorile sunt INT (nu string)?
```

---

## ?? Test Rapid în Aplica?ie

Dup? modific?ri, verific? în browser console (F12):
```javascript
// La înc?rcarea paginii:
console.log('Map data loaded:', window.mapData.length, 'regions');

// Dup? click pe raion, ar trebui s? vezi:
{
  success: true,
  regionName: "Chi?in?u",
  genderStats: [
    { gender: "Barbati", voterCount: 1234, percentage: 48.5, color: "#3498db" },
    { gender: "Femei", voterCount: 1456, percentage: 51.5, color: "#e74c3c" }
  ],
  ageStats: [
    { ageCategoryName: "18-25 ani", voterCount: 345, percentage: 12.8, color: "#e3f2fd" },
    ...
  ]
}
```

---

## ?? Dac? apar erori

### Eroare: "Invalid column name 'X'"
? SP returneaz? coloane cu nume gre?it

### Eroare: "Unable to cast object of type 'System.String' to type 'System.Int32'"
? SP returneaz? Gender sau AgeCategoryId ca STRING în loc de INT

### Eroare: "Timeout"
? SP dureaz? prea mult, verific? index-uri (vezi `STORED_PROCEDURES_DIAGNOSTIC.md`)

### Eroare: "Data is null"
? SP returneaz? NULL în loc de valori, verific? WHERE clause

---

## ?? Not? Final?

**Codul C# NU trebuie modificat** dac? stored procedures returneaz? exact structura de mai sus.

Dapper face mapare automat?:
- `RegionId` (SP) ? `RegionId` (C# property)
- `VotedCount` (SP) ? `g.VotedCount` (dynamic)
- etc.

Singura transformare manual? este `Gender` (int ? string) ?i calcularea `Percentage`.
