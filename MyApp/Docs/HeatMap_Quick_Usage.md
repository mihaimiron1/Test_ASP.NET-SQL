# Quick Guide - Heat Map Region Click Statistics

## ?? What's New

Click any **blue region** on the heat map ? See **gender & age statistics** on the right!

---

## ?? What You'll See

### **When You Click a Region (e.g., Chi?in?u):**

```
????????????????????????????????????????????????????????
?  MAP (Left)          ?  STATISTICS (Right)          ?
?                      ?                               ?
?  ??????????????     ?  CHI?IN?U                    ?
?  ?            ?     ?  45,678 votan?i              ?
?  ?  MOLDOVA   ?     ?                               ?
?  ?   [BLUE]   ?     ?  ?? Gender Statistics         ?
?  ?  Chi?in?u  ?     ?  ?? Pie Chart:               ?
?  ?  selected  ?     ?     B?rba?i: 38%             ?
?  ?            ?     ?     Femei: 62%               ?
?  ??????????????     ?                               ?
?                      ?  ?? Age Statistics            ?
?                      ?  ?? Bar Chart:               ?
?                      ?     18-25: 5,234             ?
?                      ?     26-35: 7,845             ?
?                      ?     36-45: 8,956             ?
?                      ?     46-55: 9,123             ?
?                      ?     56-65: 7,456             ?
?                      ?     66-75: 4,567             ?
?                      ?     76+: 2,897               ?
????????????????????????????????????????????????????????
```

---

## ??? How to Use

### **Step 1: Open Heat Map**
```
Navigate to: /Statistics/HeatMap
```

### **Step 2: Click a Region**
- Click any **blue region** on the map
- Region will turn **darker blue** (selected)

### **Step 3: View Statistics**
- Right panel appears automatically
- Shows:
  - ? Region name
  - ? Total voters
  - ? Gender pie chart + details
  - ? Age bar chart with all categories

### **Step 4: Click Another Region**
- Previous region returns to normal blue
- New region becomes selected
- Statistics update automatically

---

## ?? Chart Types

### **Gender Pie Chart**
```
    Femei
     ??
    ?  ?
   ?    ?
  ?______?
    B?rba?i
     ??
```
**Shows:** Distribution of male vs female voters

### **Age Bar Chart**
```
? ???     ? 18-25 years
? ?????   ? 26-35 years
? ??????  ? 36-45 years
? ??????  ? 46-55 years
? ?????   ? 56-65 years
? ????    ? 66-75 years
? ???     ? 76+ years
????????????????????
```
**Shows:** Distribution by age category

---

## ?? Quick Test

1. **Go to:** `/Statistics/HeatMap`
2. **Click:** Chi?in?u (should be blue)
3. **Verify:**
   - ? Right panel appears
   - ? Region name shows
   - ? Charts render correctly
   - ? Numbers make sense

---

## ?? If Something Doesn't Work

### **Charts don't appear?**
? Check browser console (F12) for errors

### **No data when clicking?**
? Verify region has votes in database:
```sql
EXEC sp_GetRaionGender @RegionId = 123;
```

### **Region not clickable?**
? Only **blue regions** (with votes) are clickable
? White regions (like Transnistria) are not interactive

---

## ?? Files Modified

| File | What Changed |
|------|--------------|
| `HeatMap.cshtml` | Added 2-column layout with statistics panel |
| `StatisticsController.cs` | Added `GetRegionStatisticsForHeatMap` API |
| `statistics-heatmap.js` | Added click handler and chart rendering |

---

## ?? Database Stored Procedures Used

When you click a region, these are called:

1. **`sp_GetRaionGender`** ? Gender statistics
2. **`sp_GetRaionAgeCategory`** ? Age statistics

---

## ? Features

- ? **Interactive map** - Click to select regions
- ? **Real-time statistics** - Fetched from database
- ? **Visual charts** - Pie chart (gender) + Bar chart (age)
- ? **Detailed breakdown** - Exact numbers and percentages
- ? **Responsive design** - Works on desktop and mobile
- ? **Cache optimization** - Fast loading with 60s cache

---

## ?? Done!

**Now when you click a region on the heat map, you'll see detailed gender and age statistics just like in the PrimariMunicipali view!**

**Similar to:** `Rezultate ? PrimariMunicipali` (same layout and functionality)
