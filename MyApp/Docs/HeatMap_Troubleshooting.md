# ?? Heat Map Not Working - Troubleshooting Guide

## Quick Fix Steps

### **Step 1: Open Diagnostics Page (2 minutes)**
```
Navigate to: /Statistics/Diagnostics
```

This page will show you:
- ? Is data being loaded from the database?
- ? Is data being passed to the view correctly?
- ? Are the API endpoints working?

### **Step 2: Check Browser Console (1 minute)**
1. Press **`F12`** to open Developer Tools
2. Go to **Console** tab
3. Navigate to `/Statistics/HeatMap`
4. Look for these messages:

**? Good messages:**
```
=== Heat Map Initialization ===
? Map data loaded: 32 regions
? Mapped region: MD-CU ? Chi?in?u | RegionId: 123
??? Total GeoJSON features: 36
?? Region WITH data: MD-CU Chi?in?u | RegionId: 123 | Voters: 45678
? Click handler attached to polygons
```

**? Bad messages:**
```
?? Region without MapId: {...}
? No dataItem on click
? Gender chart element not found!
```

### **Step 3: Test Click Event (30 seconds)**
1. Open **Console** (F12)
2. Click on a **blue region** on the map
3. You should see:
```
??? CLICK EVENT TRIGGERED!
?? Clicked region data: {...}
? Loading statistics for: Chi?in?u | ID: 123
?? Fetching from: /Statistics/GetRegionStatisticsForHeatMap?regionId=123
```

---

## Common Issues & Solutions

### ? **Issue 1: No Blue Regions (Everything is White)**

**Cause:** No data from stored procedure

**Fix:**
```sql
-- Run this in SQL Server Management Studio
EXEC sp_GetRaionVotingStatsForHeatMap;
```

**Should return:** Rows with RegionId, Name, NameRu, TotalVoters, LastUpdated

**If empty:** Check if your database has voting data:
```sql
SELECT COUNT(*) FROM AssignedVoterStatistics WHERE AssignedVoterStatus >= 5000;
```

---

### ? **Issue 2: Regions Not Clickable**

**Causes:**
1. JavaScript not loading
2. amCharts not loaded
3. Click handler not attached

**Fix:**

**A. Check JavaScript is loaded:**
- Open DevTools ? **Sources** tab
- Look for `statistics-heatmap.js`
- If missing, check the file path in `HeatMap.cshtml`

**B. Check amCharts is loaded:**
```javascript
// In console, type:
typeof am5
// Should return: "object"
```

**C. Check ApexCharts is loaded:**
```javascript
// In console, type:
typeof ApexCharts
// Should return: "function"
```

**D. Verify script order in `HeatMap.cshtml`:**
```html
<!-- These MUST be in this order: -->
<script src="https://cdn.amcharts.com/lib/5/index.js"></script>
<script src="https://cdn.amcharts.com/lib/5/map.js"></script>
<script src="https://cdn.amcharts.com/lib/5/themes/Animated.js"></script>
<script src="https://cdn.amcharts.com/lib/5/geodata/moldovaHigh.js"></script>
<script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>

<!-- Data MUST be set before the script -->
<script>
    window.mapData = @Html.Raw(mapJson);
</script>
<script src="/js/statistics-heatmap.js"></script>
```

---

### ? **Issue 3: Click Works But No Statistics Panel**

**Cause:** HTML elements missing

**Fix:**

**Check these elements exist in `HeatMap.cshtml`:**
```html
<div id="region-stats" class="hidden">...</div>
<div id="initial-message">...</div>
<div id="region-name"></div>
<div id="region-total-voters"></div>
<div id="gender-chart"></div>
<div id="age-chart"></div>
```

**Console should show:**
```
? Hiding initial message
? Showing stats panel
```

---

### ? **Issue 4: Statistics Panel Shows But Charts Don't Render**

**Cause:** ApexCharts not loaded or data format issue

**Fix:**

**A. Check ApexCharts:**
```html
<!-- In HeatMap.cshtml, verify this line exists: -->
<script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
```

**B. Check API response:**
- Open DevTools ? **Network** tab
- Click a region
- Find request to `/Statistics/GetRegionStatisticsForHeatMap`
- Check response format:

**? Correct format:**
```json
{
  "success": true,
  "regionName": "Chi?in?u",
  "genderStats": [
    {
      "gender": "B?rba?i",
      "votedCount": 17234,
      "percentage": 37.88,
      "color": "#3498db"
    },
    {
      "gender": "Femei",
      "votedCount": 28444,
      "percentage": 62.12,
      "color": "#e74c3c"
    }
  ],
  "ageStats": [...]
}
```

---

### ? **Issue 5: Click Event Not Triggering**

**Causes:**
1. Regions have `interactive: false`
2. Z-index issues
3. Another element overlapping

**Fix:**

**A. Check console output:**
After loading map, you should see:
```
? Click handler attached to polygons
```

**B. Test manually in console:**
```javascript
// In browser console:
document.getElementById('chartdiv').addEventListener('click', function() {
    console.log('DIV CLICKED!');
});
```
Then click the map. If you see "DIV CLICKED!" the map area is receiving clicks.

**C. Verify no CSS issues:**
```javascript
// Check if map is visible:
const mapDiv = document.getElementById('chartdiv');
console.log('Map visible:', mapDiv.offsetWidth, 'x', mapDiv.offsetHeight);
// Should show: Map visible: 600 x 600 (or similar)
```

---

## ?? Debugging Checklist

Run through this checklist:

### **Database:**
- [ ] `EXEC sp_GetRaionVotingStatsForHeatMap` returns rows
- [ ] `EXEC sp_GetRaionGender @RegionId=123` returns rows
- [ ] `EXEC sp_GetRaionAgeCategory @RegionId=123` returns rows

### **Backend:**
- [ ] `/Statistics/DebugMapData` shows regions with MapIds
- [ ] `/Statistics/GetRegionStatisticsForHeatMap?regionId=123` returns JSON

### **Frontend:**
- [ ] `/Statistics/HeatMap` loads without errors
- [ ] Browser console shows "=== Heat Map Initialization ==="
- [ ] Blue regions visible on map
- [ ] Console shows "? Click handler attached"
- [ ] Clicking region shows "??? CLICK EVENT TRIGGERED!"

### **Scripts:**
- [ ] amCharts loaded (`typeof am5` returns `"object"`)
- [ ] ApexCharts loaded (`typeof ApexCharts` returns `"function"`)
- [ ] `window.mapData` is defined and contains regions

---

## ??? Quick SQL Test

Run this to verify database setup:

```sql
-- 1. Check if stored procedures exist
SELECT name FROM sys.procedures 
WHERE name LIKE 'sp_Get%Statistics%' 
ORDER BY name;

-- Should show:
-- sp_GetRaionVotingStatsForHeatMap
-- sp_GetRaionGender
-- sp_GetRaionAgeCategory
-- sp_GetLocalitateGender
-- sp_GetLocalitateAgeCategory

-- 2. Test heat map SP
EXEC sp_GetRaionVotingStatsForHeatMap;
-- Should return rows with voting data

-- 3. Get a RegionId to test with
SELECT TOP 1 RegionId, Name FROM dbo.Region 
WHERE RegionTypeId IN (2,3,4)
ORDER BY Name;

-- 4. Test gender SP with that RegionId
EXEC sp_GetRaionGender @RegionId = 123; -- Replace 123 with actual ID

-- 5. Test age SP with that RegionId
EXEC sp_GetRaionAgeCategory @RegionId = 123; -- Replace 123 with actual ID
```

---

## ?? Step-by-Step Test Procedure

### **Test 1: Database**
```sql
EXEC sp_GetRaionVotingStatsForHeatMap;
```
**Expected:** Rows with region data  
**If fails:** Run `CreateStoredProcedures.sql` script

### **Test 2: Backend API**
```
Open browser: /Statistics/DebugMapData
```
**Expected:** JSON with regions and MapIds  
**If fails:** Check controller code

### **Test 3: Frontend View**
```
Open browser: /Statistics/HeatMap
```
**Expected:** Map loads with blue regions  
**If fails:** Check console for errors

### **Test 4: Click Event**
```
1. F12 ? Console
2. Click blue region
3. Look for: "??? CLICK EVENT TRIGGERED!"
```
**Expected:** Console shows click event  
**If fails:** Check JavaScript is loaded

### **Test 5: Statistics API**
```
Console: 
fetch('/Statistics/GetRegionStatisticsForHeatMap?regionId=123')
  .then(r => r.json())
  .then(d => console.log(d))
```
**Expected:** JSON with gender and age stats  
**If fails:** Check stored procedures

---

## ?? Force Refresh Everything

If nothing works, try this:

1. **Clear browser cache:** `Ctrl + Shift + Delete`
2. **Restart application:** `Shift + F5` in VS, then `F5`
3. **Hard refresh page:** `Ctrl + F5`
4. **Check app is running:** Look for "Application started" in console

---

## ?? Still Not Working?

### **Collect this information:**

1. **Browser console output** (copy all red errors)
2. **Network tab** (check for failed requests)
3. **Database test results** (run SQL queries above)
4. **Screenshots** of:
   - Map page (showing white/empty map)
   - Browser console (with errors)
   - Network tab (showing failed API calls)

Then check:
- Is the application running? (port 5000 or 5001)
- Is SQL Server running?
- Are you using the correct URL?

---

## ? Success Checklist

When everything works, you should see:

- [x] Map loads with **blue regions**
- [x] Console shows **"? Click handler attached"**
- [x] Click region ? Console shows **"??? CLICK EVENT TRIGGERED!"**
- [x] Right panel appears with **region name**
- [x] **Pie chart** renders (gender)
- [x] **Bar chart** renders (age)
- [x] **No red errors** in console

**If all checked ? ? IT WORKS!** ??
