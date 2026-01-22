// =========================================================
// Moldova GeoJSON Map IDs Reference
// =========================================================
// These are the IDs used in moldovaHigh.js GeoJSON file
// You need to map your database RegionId to these MapIds
// =========================================================

const MOLDOVA_GEOJSON_IDS = {
    // Format: "MapId": "Region Name"
    
    // IMPORTANT: These are the actual IDs in the moldovaHigh.js file
    // Your database RegionId must map to one of these
    
    "MD-AN": "Anenii Noi",
    "MD-BA": "Basarabeasca",
    "MD-BD": "Tighina (Bender)",
    "MD-BR": "Briceni",
    "MD-BS": "B?l?i",
    "MD-CA": "Cahul",
    "MD-CL": "C?l?ra?i",
    "MD-CM": "Cimi?lia",
    "MD-CR": "Criuleni",
    "MD-CS": "C?u?eni",
    "MD-CT": "Cantemir",
    "MD-CU": "Chi?in?u",
    "MD-DO": "Dondu?eni",
    "MD-DR": "Drochia",
    "MD-DU": "Dub?sari",
    "MD-ED": "Edine?",
    "MD-FA": "F?le?ti",
    "MD-FL": "Flore?ti",
    "MD-GA": "G?g?uzia",
    "MD-GL": "Glodeni",
    "MD-HI": "Hînce?ti",
    "MD-IA": "Ialoveni",
    "MD-LE": "Leova",
    "MD-NI": "Nisporeni",
    "MD-OC": "Ocni?a",
    "MD-OR": "Orhei",
    "MD-RE": "Rezina",
    "MD-RI": "Rî?cani",
    "MD-SD": "?old?ne?ti",
    "MD-SI": "Sîngerei",
    "MD-SO": "Soroca",
    "MD-ST": "Str??eni",
    "MD-SV": "?tefan Vod?",
    "MD-TA": "Taraclia",
    "MD-TE": "Telene?ti",
    "MD-UN": "Ungheni",
    
    // TRANSNISTRIA (not controlled - will be white):
    // "MD-SN": "Stînga Nistrului",
    // These regions are in GeoJSON but not in our mapper
};

// HOW TO USE THIS:
// 1. Run GetRegionMappings.sql to get your database RegionIds
// 2. Match your database Region.Name to the names above
// 3. Create mapping: database RegionId ? MapId (like MD-CU)
// 4. Add to RegionMapIdMapper.cs

// EXAMPLE:
// If your database shows:
//   RegionId = 5, Name = "Chi?in?u"
// Then map:
//   { 5, "MD-CU" }  // in RegionIdToMapId dictionary
