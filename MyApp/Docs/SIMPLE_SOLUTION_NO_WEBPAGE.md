# ?? SIMPLE SOLUTION - 3 Steps (5 Minutes)

## ? The GenerateMapperCode page isn't loading because of port mismatch

Your app runs on port **5165** (not 5000), so the correct URL is:
```
http://localhost:5165/Statistics/GenerateMapperCode
```

But here's an **EASIER** solution that doesn't require the web page:

---

## ?? **QUICK FIX (No Web Page Needed)**

### **Step 1: Run SQL Query** (1 minute)

1. Open **SQL Server Management Studio**
2. Connect to your database
3. Run this query:

```sql
SELECT 
    r.RegionId,
    r.Name AS RegionName,
    CASE 
        WHEN r.Name LIKE '%Chi?in?u%' OR r.Name LIKE '%Chisinau%' THEN 'MD-CU'
        WHEN r.Name LIKE '%B?l?i%' OR r.Name LIKE '%Balti%' THEN 'MD-BS'
        WHEN r.Name LIKE '%Cahul%' THEN 'MD-CA'
        WHEN r.Name LIKE '%Orhei%' THEN 'MD-OR'
        WHEN r.Name LIKE '%Soroca%' THEN 'MD-SO'
        WHEN r.Name LIKE '%Ungheni%' THEN 'MD-UN'
        WHEN r.Name LIKE '%Edine?%' OR r.Name LIKE '%Edinet%' THEN 'MD-ED'
        WHEN r.Name LIKE '%Anenii Noi%' THEN 'MD-AN'
        WHEN r.Name LIKE '%Basarabeasca%' THEN 'MD-BA'
        WHEN r.Name LIKE '%Briceni%' THEN 'MD-BR'
        WHEN r.Name LIKE '%C?l?ra?i%' OR r.Name LIKE '%Calarasi%' THEN 'MD-CL'
        WHEN r.Name LIKE '%Cantemir%' THEN 'MD-CT'
        WHEN r.Name LIKE '%C?u?eni%' OR r.Name LIKE '%Causeni%' THEN 'MD-CS'
        WHEN r.Name LIKE '%Cimi?lia%' OR r.Name LIKE '%Cimislia%' THEN 'MD-CM'
        WHEN r.Name LIKE '%Criuleni%' THEN 'MD-CR'
        WHEN r.Name LIKE '%Dondu?eni%' OR r.Name LIKE '%Donduseni%' THEN 'MD-DO'
        WHEN r.Name LIKE '%Drochia%' THEN 'MD-DR'
        WHEN r.Name LIKE '%Dub?sari%' OR r.Name LIKE '%Dubasari%' THEN 'MD-DU'
        WHEN r.Name LIKE '%F?le?ti%' OR r.Name LIKE '%Falesti%' THEN 'MD-FA'
        WHEN r.Name LIKE '%Flore?ti%' OR r.Name LIKE '%Floresti%' THEN 'MD-FL'
        WHEN r.Name LIKE '%Glodeni%' THEN 'MD-GL'
        WHEN r.Name LIKE '%Hînce?ti%' OR r.Name LIKE '%Hincesti%' THEN 'MD-HI'
        WHEN r.Name LIKE '%Ialoveni%' THEN 'MD-IA'
        WHEN r.Name LIKE '%Leova%' THEN 'MD-LE'
        WHEN r.Name LIKE '%Nisporeni%' THEN 'MD-NI'
        WHEN r.Name LIKE '%Ocni?a%' OR r.Name LIKE '%Ocnita%' THEN 'MD-OC'
        WHEN r.Name LIKE '%Rezina%' THEN 'MD-RE'
        WHEN r.Name LIKE '%Rî?cani%' OR r.Name LIKE '%Riscani%' THEN 'MD-RI'
        WHEN r.Name LIKE '%Sîngerei%' OR r.Name LIKE '%Singerei%' THEN 'MD-SI'
        WHEN r.Name LIKE '%Str??eni%' OR r.Name LIKE '%Straseni%' THEN 'MD-ST'
        WHEN r.Name LIKE '%?old?ne?ti%' OR r.Name LIKE '%Soldanesti%' THEN 'MD-SD'
        WHEN r.Name LIKE '%?tefan Vod?%' OR r.Name LIKE '%Stefan Voda%' THEN 'MD-SV'
        WHEN r.Name LIKE '%Taraclia%' THEN 'MD-TA'
        WHEN r.Name LIKE '%Telene?ti%' OR r.Name LIKE '%Telenesti%' THEN 'MD-TE'
        WHEN r.Name LIKE '%Tighina%' OR r.Name LIKE '%Bender%' THEN 'MD-BD'
        ELSE NULL
    END AS MapId
FROM dbo.Region r
WHERE r.RegionTypeId IN (2, 3, 4)
  AND EXISTS (SELECT 1 FROM dbo.AssignedVoterStatistics avs WHERE avs.RegionId = r.RegionId)
ORDER BY r.Name;
```

4. **Copy the results** (you'll see RegionId, RegionName, MapId)

---

### **Step 2: Format as C# Code** (2 minutes)

Take each row from the SQL results and format like this:

**SQL Result:**
```
RegionId | RegionName  | MapId
---------|-------------|-------
5        | Chi?in?u    | MD-CU
12       | B?l?i       | MD-BS
23       | Cahul       | MD-CA
```

**Format as C# (copy this pattern):**
```csharp
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    { 5, "MD-CU" },   // Chi?in?u
    { 12, "MD-BS" },  // B?l?i
    { 23, "MD-CA" },  // Cahul
    // ... add all your rows here
};
```

---

### **Step 3: Update RegionMapIdMapper.cs** (2 minutes)

1. Open file: `MyApp\Services\RegionMapIdMapper.cs`

2. Find this section (around line 15-25):

```csharp
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    // TODO: Add mappings here
};
```

3. Replace with your formatted code from Step 2

4. **Save the file** (Ctrl + S)

5. **Rebuild** (Ctrl + Shift + B)

6. **Restart app** (Shift + F5, then F5)

7. **Test:** Go to `http://localhost:5165/Statistics/HeatMap`

---

## ?? **Example of Final Code**

```csharp
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    // Your actual RegionIds from database:
    { 5, "MD-CU" },   // Chi?in?u
    { 12, "MD-BS" },  // B?l?i
    { 23, "MD-CA" },  // Cahul
    { 34, "MD-OR" },  // Orhei
    { 45, "MD-SO" },  // Soroca
    { 56, "MD-UN" },  // Ungheni
    { 67, "MD-ED" },  // Edine?
    { 78, "MD-AN" },  // Anenii Noi
    { 89, "MD-BA" },  // Basarabeasca
    { 90, "MD-BR" },  // Briceni
    { 91, "MD-CL" },  // C?l?ra?i
    { 92, "MD-CT" },  // Cantemir
    { 93, "MD-CS" },  // C?u?eni
    { 94, "MD-CM" },  // Cimi?lia
    { 95, "MD-CR" },  // Criuleni
    { 96, "MD-DO" },  // Dondu?eni
    { 97, "MD-DR" },  // Drochia
    { 98, "MD-DU" },  // Dub?sari
    { 99, "MD-FA" },  // F?le?ti
    { 100, "MD-FL" }, // Flore?ti
    { 101, "MD-GL" }, // Glodeni
    { 102, "MD-HI" }, // Hînce?ti
    { 103, "MD-IA" }, // Ialoveni
    { 104, "MD-LE" }, // Leova
    { 105, "MD-NI" }, // Nisporeni
    { 106, "MD-OC" }, // Ocni?a
    { 107, "MD-RE" }, // Rezina
    { 108, "MD-RI" }, // Rî?cani
    { 109, "MD-SI" }, // Sîngerei
    { 110, "MD-ST" }, // Str??eni
    { 111, "MD-SD" }, // ?old?ne?ti
    { 112, "MD-SV" }, // ?tefan Vod?
    { 113, "MD-TA" }, // Taraclia
    { 114, "MD-TE" }, // Telene?ti
    { 115, "MD-BD" }, // Tighina/Bender
};
```

*(Replace the IDs with your actual database IDs from Step 1)*

---

## ? **Success Check**

After Step 3, navigate to:
```
http://localhost:5165/Statistics/DebugMapData
```

**You should see:**
```json
{
  "regionsWithMapId": 32,     // Should be > 0
  "regionsWithoutMapId": 0,   // Should be 0
}
```

Then test the heat map:
```
http://localhost:5165/Statistics/HeatMap
```

**Click a region ? Statistics should appear!** ?

---

## ??? **Alternative: Use Helper Files**

I created two helper files for you:

1. **SQL Script:** `MyApp\Scripts\SOLUTION_GenerateMapping.sql`
   - Open in SSMS and execute
   - Shows suggested MapIds for each region

2. **PowerShell Script:** `MyApp\Scripts\GenerateMapperCode.ps1`
   - Right-click ? Run with PowerShell
   - Paste SQL results
   - Generates formatted C# code

---

## ?? **Summary**

**Problem:** RegionId (numbers) ? MapId (strings like "MD-CU")

**Solution:** Create a dictionary mapping them

**Time:** 5 minutes

**Files to edit:** Only `RegionMapIdMapper.cs`

**Result:** Heat map clicks work! ??

---

**Do Step 1 now - run the SQL query and copy the results!**
