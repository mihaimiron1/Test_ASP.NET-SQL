# ??? Fix Heat Map - Region ID Mapping Solution

## ?? THE PROBLEM

**Database RegionIds** (like 1, 2, 3, 47, 123...) **DON'T MATCH** **GeoJSON MapIds** (like "MD-CU", "MD-BS"...)

**Example:**
- Database: `RegionId = 5, Name = "Chi?in?u"`
- GeoJSON: `id = "MD-CU", name = "Chi?in?u"`
- **They don't match!** So clicks don't work.

---

## ? THE SOLUTION (Choose One)

### **Option 1: Automatic Mapping (RECOMMENDED)** ?

Uses a code generator page to create the mapping automatically.

**Steps:**

1. **Stop the app** (Shift + F5)

2. **Start the app** (F5)

3. **Navigate to:**
```
http://localhost:5000/Statistics/GenerateMapperCode
```

4. **You'll see:**
   - Table showing all your database regions
   - Which ones are mapped (?) and which aren't (?)
   - Generated C# code to copy

5. **Copy the generated code** (click "?? Copy to Clipboard" button)

6. **Open file:**
```
MyApp\Services\RegionMapIdMapper.cs
```

7. **Replace the `RegionIdToMapId` dictionary:**

```csharp
// BEFORE (empty or incomplete):
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    // Add mappings here if you know RegionId values
};

// AFTER (paste the generated code):
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    { 5, "MD-CU" },   // Chi?in?u
    { 47, "MD-AN" },  // Anenii Noi
    { 123, "MD-BS" }, // B?l?i
    // ... all your regions
};
```

8. **Save the file**

9. **Rebuild** (Ctrl + Shift + B)

10. **Test:** Navigate to `/Statistics/HeatMap` and click regions!

---

### **Option 2: Manual SQL Lookup**

If you prefer to do it manually:

**Step 1: Get your database RegionIds**

Run this SQL:

```sql
SELECT 
    r.RegionId,
    r.Name AS RegionName,
    r.NameRu AS RegionNameRu
FROM dbo.Region r
INNER JOIN dbo.AssignedVoterStatistics avs ON avs.RegionId = r.RegionId
WHERE r.RegionTypeId IN (2, 3, 4)
GROUP BY r.RegionId, r.Name, r.NameRu
ORDER BY r.Name;
```

**Step 2: Match to GeoJSON MapIds**

Use this reference table:

| Database Name | GeoJSON MapId |
|---------------|---------------|
| Anenii Noi | MD-AN |
| Basarabeasca | MD-BA |
| B?l?i | MD-BS |
| Cahul | MD-CA |
| C?l?ra?i | MD-CL |
| Cantemir | MD-CT |
| C?u?eni | MD-CS |
| Chi?in?u | MD-CU |
| Cimi?lia | MD-CM |
| Criuleni | MD-CR |
| Dondu?eni | MD-DO |
| Drochia | MD-DR |
| Dub?sari | MD-DU |
| Edine? | MD-ED |
| F?le?ti | MD-FA |
| Flore?ti | MD-FL |
| G?g?uzia | MD-GA |
| Glodeni | MD-GL |
| Hînce?ti | MD-HI |
| Ialoveni | MD-IA |
| Leova | MD-LE |
| Nisporeni | MD-NI |
| Ocni?a | MD-OC |
| Orhei | MD-OR |
| Rezina | MD-RE |
| Rî?cani | MD-RI |
| Sîngerei | MD-SI |
| Soroca | MD-SO |
| Str??eni | MD-ST |
| ?old?ne?ti | MD-SD |
| ?tefan Vod? | MD-SV |
| Taraclia | MD-TA |
| Telene?ti | MD-TE |
| Tighina (Bender) | MD-BD |
| Ungheni | MD-UN |

**Step 3: Create the mapping**

```csharp
// Example based on your SQL results:
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    // Format: { DatabaseRegionId, "GeoJSON-MapId" },
    
    { 5, "MD-CU" },   // Chi?in?u
    { 12, "MD-BS" },  // B?l?i
    { 23, "MD-CA" },  // Cahul
    { 34, "MD-OR" },  // Orhei
    { 45, "MD-SO" },  // Soroca
    { 56, "MD-UN" },  // Ungheni
    { 47, "MD-AN" },  // Anenii Noi
    { 78, "MD-ED" },  // Edine?
    // ... add all your regions here
};
```

**Step 4: Paste into `RegionMapIdMapper.cs`**

**Step 5: Save, rebuild, test!**

---

## ?? How to Verify It Works

### **Test 1: Check DebugMapData**

```
http://localhost:5000/Statistics/DebugMapData
```

**Look for:**
```json
{
  "regionsWithMapId": 32,  // Should be > 0
  "regionsWithoutMapId": 0, // Should be 0
  "details": [
    {
      "databaseRegionId": 5,
      "databaseRegionName": "Chi?in?u",
      "mapId": "MD-CU",  // ? Should NOT be null
      "mappingStatus": "? Will be colored on map"
    }
  ]
}
```

### **Test 2: Check Browser Console**

Navigate to `/Statistics/HeatMap` and press F12:

**Before fix (broken):**
```
?? Region without MapId: {...}
? Total regions mapped: 0
```

**After fix (working):**
```
? Mapped region: MD-CU ? Chi?in?u | RegionId: 5
? Mapped region: MD-BS ? B?l?i | RegionId: 12
?? Total regions mapped: 32
?? Region WITH data: MD-CU Chi?in?u | RegionId: 5 | Voters: 45678
```

### **Test 3: Click a Region**

Click a blue region on the map:

**Should see:**
```
??? CLICK EVENT TRIGGERED!
?? Clicked region data: {id: "MD-CU", regionId: 5, hasData: true}
? Loading statistics for: Chi?in?u | ID: 5
```

**Right panel should appear with charts!** ?

---

## ?? What the Mapping Does

```
DATABASE                    GEOJSON MAP
????????????????           ????????????????
? RegionId: 5  ?           ? id: "MD-CU"  ?
? Name: Chi?in?u? ?????????? name: Chi?in?u?
????????????????           ????????????????
        ?                          ?
        ????????? MAPPING ??????????
           { 5, "MD-CU" }
```

**Without mapping:** Click does nothing (IDs don't match)  
**With mapping:** Click works! (IDs are translated)

---

## ??? Files to Modify

### **Only ONE file needs to be edited:**

```
MyApp\Services\RegionMapIdMapper.cs
```

**What to change:**

```csharp
// Find this section:
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    // Add mappings here if you know RegionId values
    // Example: { 1, "MD-AN" },
};

// Replace with your mappings:
private static readonly Dictionary<int, string> RegionIdToMapId = new()
{
    { 5, "MD-CU" },   // Chi?in?u
    { 12, "MD-BS" },  // B?l?i
    { 23, "MD-CA" },  // Cahul
    // ... (use the generator page or manual lookup)
};
```

**That's it!** No database changes, no GeoJSON changes, just this one dictionary.

---

## ?? Quick Start (5 Minutes)

1. **Stop app** ? **Start app** (Shift + F5, then F5)
2. **Go to:** `/Statistics/GenerateMapperCode`
3. **Copy the generated code** (click button)
4. **Open:** `RegionMapIdMapper.cs`
5. **Paste** the code into `RegionIdToMapId` dictionary
6. **Save** ? **Rebuild** (Ctrl + Shift + B)
7. **Test:** `/Statistics/HeatMap` ? **Click a region!**

---

## ? Success Checklist

After adding the mapping:

- [ ] `/Statistics/DebugMapData` shows `regionsWithMapId > 0`
- [ ] `/Statistics/HeatMap` shows blue regions
- [ ] Console shows "? Mapped region: MD-CU ? Chi?in?u"
- [ ] Clicking region shows "??? CLICK EVENT TRIGGERED!"
- [ ] Right panel appears with region name
- [ ] Charts render (gender pie chart, age bar chart)
- [ ] No errors in console

**If all checked ? ? IT WORKS!** ??

---

## ?? Common Issues After Mapping

### **Issue: Still not working after adding mapping**

**Fix:**
1. Make sure you saved the file
2. Rebuild the project (Ctrl + Shift + B)
3. Restart the app (Shift + F5, then F5)
4. Hard refresh browser (Ctrl + F5)

### **Issue: Some regions work, others don't**

**Fix:**
- Check `/Statistics/GenerateMapperCode`
- Look for regions with ? status
- Add those mappings too

### **Issue: "RegionId X not found"**

**Fix:**
- That RegionId doesn't exist in your database
- Remove it from the mapping or check your SQL query

---

## ?? Summary

**Problem:** Database IDs (1, 2, 3...) ? Map IDs ("MD-CU", "MD-BS"...)  
**Solution:** Create a dictionary that maps them  
**Tool:** Use `/Statistics/GenerateMapperCode` to auto-generate  
**File:** Edit `RegionMapIdMapper.cs` only  
**Result:** Clicks work! Statistics show! ??

**This is the correct and recommended approach!** ?
