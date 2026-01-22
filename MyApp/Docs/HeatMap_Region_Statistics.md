# Heat Map with Region Statistics - Implementation Guide

## ?? Overview

When you click on a region in the heat map, it now displays detailed statistics about gender and age distribution on the right side, similar to the PrimariMunicipali view.

---

## ?? Features Implemented

### **1. Two-Column Layout**
- **Left:** Interactive heat map of Moldova
- **Right:** Statistics panel (hidden by default, shown when region is clicked)

### **2. Statistics Displayed**
When you click a region, you'll see:
- ? **Region name** (e.g., "Chi?in?u")
- ? **Total voters** count
- ? **Gender statistics** (Pie chart + details)
  - B?rba?i (Men) - Blue
  - Femei (Women) - Red
  - Percentage breakdown
- ? **Age category statistics** (Bar chart)
  - 18-25 ani
  - 26-35 ani
  - 36-45 ani
  - 46-55 ani
  - 56-65 ani
  - 66-75 ani
  - 76+ ani

---

## ?? Technical Implementation

### **1. View Changes** (`HeatMap.cshtml`)

**Before:**
```razor
<div id="chartdiv" style="width: 100%; height: 600px;"></div>
```

**After:**
```razor
<div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
    <!-- Map on left -->
    <div id="chartdiv"></div>
    
    <!-- Statistics on right -->
    <div id="region-stats" class="hidden">
        <!-- Gender Chart -->
        <!-- Age Chart -->
    </div>
</div>
```

### **2. Controller Changes** (`StatisticsController.cs`)

**New API Endpoint:**
```csharp
[HttpGet]
public async Task<IActionResult> GetRegionStatisticsForHeatMap(int regionId)
{
    // Calls sp_GetRaionGender and sp_GetRaionAgeCategory
    var stats = await _repository.GetRegionDetailedStatisticsAsync(regionId);
    
    return Json(new {
        success = true,
        regionName = stats.RegionName,
        totalVoters = stats.TotalVoters,
        genderStats = stats.GenderStats,
        ageStats = stats.AgeStats
    });
}
```

**What it does:**
1. Gets `regionId` from the map click
2. Calls `GetRegionDetailedStatisticsAsync` (which uses stored procedures)
3. Returns JSON with gender and age statistics

### **3. JavaScript Changes** (`statistics-heatmap.js`)

**New Functionality:**

#### **A. Click Handler**
```javascript
polygonSeries.mapPolygons.template.events.on("click", function (ev) {
    var regionId = ev.target.dataItem.dataContext.regionId;
    loadRegionStatistics(regionId);
});
```

#### **B. Load Statistics**
```javascript
function loadRegionStatistics(regionId) {
    fetch(`/Statistics/GetRegionStatisticsForHeatMap?regionId=${regionId}`)
        .then(response => response.json())
        .then(data => {
            renderGenderChart(data.genderStats);
            renderAgeChart(data.ageStats);
        });
}
```

#### **C. Render Charts**
Uses **ApexCharts** library to render:
- **Pie Chart** for gender distribution
- **Bar Chart** for age distribution

---

## ??? Database Stored Procedures Used

### **1. sp_GetRaionGender**
```sql
-- Called when region is clicked
EXEC sp_GetRaionGender @RegionId = 123
```

**Returns:**
| Gender | VoterCount | VotedCount |
|--------|------------|------------|
| 1 (B?rba?i) | 15000 | 12000 |
| 2 (Femei) | 18000 | 15000 |

### **2. sp_GetRaionAgeCategory**
```sql
-- Called when region is clicked
EXEC sp_GetRaionAgeCategory @RegionId = 123
```

**Returns:**
| AgeCategoryId | VoterCount |
|---------------|------------|
| 1 (18-25) | 5000 |
| 2 (26-35) | 7000 |
| 3 (36-45) | 8000 |
| ... | ... |

---

## ?? User Flow

### **Step 1: Initial View**
```
???????????????????????????????????????
?  Heat Map                           ?
?  ?????????????????  ??????????????? ?
?  ?               ?  ? "Selecta?i  ? ?
?  ?   MOLDOVA     ?  ?  un raion   ? ?
?  ?     MAP       ?  ?  pentru     ? ?
?  ?               ?  ?  statistici"? ?
?  ?????????????????  ??????????????? ?
???????????????????????????????????????
```

### **Step 2: User Clicks Region**
```
User clicks "Chi?in?u" on map
         ?
JavaScript captures click event
         ?
Gets regionId from clicked polygon
         ?
Calls /Statistics/GetRegionStatisticsForHeatMap?regionId=123
```

### **Step 3: API Processes Request**
```
Controller receives regionId
         ?
Calls GetRegionDetailedStatisticsAsync(123)
         ?
Repository executes:
  - sp_GetRaionGender @RegionId=123
  - sp_GetRaionAgeCategory @RegionId=123
         ?
Returns JSON with statistics
```

### **Step 4: Statistics Displayed**
```
???????????????????????????????????????
?  Heat Map                           ?
?  ?????????????  ??????????????????? ?
?  ?           ?  ? CHI?IN?U        ? ?
?  ? MOLDOVA   ?  ? 45,678 votan?i  ? ?
?  ?   MAP     ?  ?                 ? ?
?  ?  [BLUE]   ?  ? ?? Gender Chart ? ?
?  ?           ?  ? ?? Age Chart    ? ?
?  ?????????????  ??????????????????? ?
???????????????????????????????????????
```

---

## ?? Visual Appearance

### **Gender Statistics (Pie Chart)**
```
     Femei (62%)
        ??
       ?  ?
      ?    ?
     ?      ?
    ?        ?
   ?          ?
  ??????????????
    B?rba?i (38%)
       ??

+ Details Table:
????????????????????????????
? ?? B?rba?i ? 17,234 (38%)?
? ?? Femei   ? 28,444 (62%)?
????????????????????????????
```

### **Age Statistics (Bar Chart)**
```
    Votan?i
      ?
8000  ?     ???
      ?     ???  ???
6000  ? ??? ???  ???
      ? ??? ???  ??? ???
4000  ? ??? ???  ??? ??? ???
      ? ??? ???  ??? ??? ??? ???
2000  ? ??? ???  ??? ??? ??? ??? ???
      ???????????????????????????????
       18-25 26-35 36-45 46-55 56-65 66-75 76+
```

---

## ?? How to Test

### **1. Navigate to Heat Map**
```
/Statistics/HeatMap
```

### **2. Check Initial State**
- ? Map loads with colored regions
- ? Right panel shows "Selecta?i un raion..."
- ? White regions (Transnistria) are not clickable

### **3. Click a Blue Region**
- ? Region turns darker blue (selected)
- ? Right panel appears with statistics
- ? Gender pie chart loads
- ? Age bar chart loads
- ? Details show correct numbers

### **4. Click Another Region**
- ? Previous region returns to normal blue
- ? New region turns dark blue
- ? Statistics update for new region

### **5. Hover Over Regions**
- ? Blue regions show tooltip with voter count
- ? White regions show "(Nu au votat)"

---

## ?? Troubleshooting

### **Issue: Charts Don't Appear**
**Cause:** ApexCharts library not loaded

**Solution:** Check HeatMap.cshtml has:
```html
<script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
```

### **Issue: No Statistics When Clicking**
**Cause 1:** API endpoint not returning data

**Check:**
```javascript
// Open browser console (F12)
// Click a region
// Check Network tab for API call
// Should see: /Statistics/GetRegionStatisticsForHeatMap?regionId=123
```

**Cause 2:** Region not in database

**Check:**
```sql
-- Verify region has data
EXEC sp_GetRaionGender @RegionId = 123;
EXEC sp_GetRaionAgeCategory @RegionId = 123;
```

### **Issue: Wrong Statistics Displayed**
**Cause:** Cache showing old data

**Solution:**
```csharp
// Clear cache or wait 60 seconds
// Or restart application
```

---

## ?? API Response Example

**Request:**
```
GET /Statistics/GetRegionStatisticsForHeatMap?regionId=123
```

**Response:**
```json
{
  "success": true,
  "regionId": 123,
  "regionName": "Chi?in?u",
  "totalVoters": 45678,
  "votedCount": 38456,
  "votingPercentage": 84.19,
  "genderStats": [
    {
      "gender": "B?rba?i",
      "voterCount": 17234,
      "votedCount": 14567,
      "percentage": 37.88,
      "color": "#3498db"
    },
    {
      "gender": "Femei",
      "voterCount": 28444,
      "votedCount": 23889,
      "percentage": 62.12,
      "color": "#e74c3c"
    }
  ],
  "ageStats": [
    {
      "ageCategoryId": 1,
      "ageCategoryName": "18-25 ani",
      "voterCount": 5234,
      "percentage": 13.61,
      "color": "#e3f2fd"
    },
    {
      "ageCategoryId": 2,
      "ageCategoryName": "26-35 ani",
      "voterCount": 7845,
      "percentage": 20.41,
      "color": "#bbdefb"
    }
    // ... more age categories
  ],
  "lastUpdated": "2024-01-15T10:30:00"
}
```

---

## ? Verification Checklist

- [ ] Navigate to `/Statistics/HeatMap`
- [ ] Map loads with colored regions
- [ ] Right panel shows initial message
- [ ] Click on a blue region (e.g., Chi?in?u)
- [ ] Region turns dark blue (selected state)
- [ ] Right panel appears with region name
- [ ] Total voters count displays correctly
- [ ] Gender pie chart renders with correct data
- [ ] Gender details table shows percentages
- [ ] Age bar chart renders with all 7 categories
- [ ] Hover over bars shows tooltips with percentages
- [ ] Click another region - statistics update
- [ ] Previous region returns to normal blue
- [ ] White regions (Transnistria) are not clickable
- [ ] Console shows no JavaScript errors

---

## ?? Summary

**What Changed:**
1. ? HeatMap view now has 2-column layout
2. ? Added API endpoint `GetRegionStatisticsForHeatMap`
3. ? JavaScript handles click events and fetches data
4. ? ApexCharts renders gender (pie) and age (bar) charts
5. ? Uses existing stored procedures (sp_GetRaionGender, sp_GetRaionAgeCategory)

**What Happens When You Click:**
1. Click region ? Get regionId
2. API call ? Fetch statistics
3. Render charts ? Display data

**Just like PrimariMunicipali view, but for voting statistics!** ??
