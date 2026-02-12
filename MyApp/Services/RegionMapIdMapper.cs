namespace MyApp.Services
{
    /// <summary>
    /// Maps database RegionId to GeoJSON map IDs (MD-AN, MD-BS, etc.)
    /// Regions not mapped here will appear WHITE on the heat map
    /// </summary>
    public static class RegionMapIdMapper
    {
        // ============================================================
        // FILL IN YOUR DATABASE REGION IDs HERE
        // Format: { databaseRegionId, "GeoJSON-MapId" }
        // 
        // Example from your database:
        //   RegionId=5 is "ANENII NOI" → map to "MD-AN"
        //   RegionId=3 is "BĂLȚI" → map to "MD-BS"
        //   RegionId=6 is "BASARABEASCA" → map to "MD-BA"
        //
        // Regions NOT in this list will remain WHITE on the map
        // ============================================================
        private static readonly Dictionary<int, string> RegionIdToMapId = new()
        {
            // Replace ??? with your database RegionId for each region:
            
            { 5, "MD-AN" },  // Anenii Noi
            { 6, "MD-BS" },  // Basarabeasca
            { 7, "MD-BR" },  // Briceni
            { 3, "MD-BA" },  // Bălți
            { 8, "MD-CA" },  // Cahul
            { 10, "MD-CL" },  // Călărași
            { 12, "MD-CM" },  // Cimișlia
            { 13, "MD-CR" },  // Criuleni
            { 11, "MD-CS" },  // Căușeni
            { 9, "MD-CT" },  // Cantemir
            { 2, "MD-CU" },  // Chișinău
            { 14, "MD-DO" },  // Dondușeni
            { 15, "MD-DR" },  // Drochia
            { 16, "MD-DU" },  // Dubăsari
            { 17, "MD-ED" },  // Edineț
            { 18, "MD-FA" },  // Fălești
            { 19, "MD-FL" },  // Florești
            { 37, "MD-GA" },  // Găgăuzia (UTA)
            { 20, "MD-GL" },  // Glodeni
            { 21, "MD-HI" },  // Hîncești
            { 22, "MD-IA" },  // Ialoveni
            { 23, "MD-LE" },  // Leova
            { 24, "MD-NI" },  // Nisporeni
            { 25, "MD-OC" },  // Ocnița
            { 26, "MD-OR" },  // Orhei
            { 27, "MD-RE" },  // Rezina
            { 28, "MD-RI" },  // Rîșcani
            { 32, "MD-SD" },  // Șoldănești
            { 29, "MD-SI" },  // Sîngerei
            { 30, "MD-SO" },  // Soroca
            { 31, "MD-ST" },  // Strășeni
            { 33, "MD-SV" },  // Ștefan Vodă
            { 34, "MD-TA" },  // Taraclia
            { 35, "MD-TE" },  // Telenești
            { 14721, "MD-SN" },  // Unitatea Teritorială din Stînga Nistrului 
            { 36, "MD-UN" },  // Ungheni
        };

        /// <summary>
        /// Gets the GeoJSON map ID for a database RegionId
        /// Returns null if region is not mapped (will appear white on map)
        /// </summary>
        public static string? GetMapId(int regionId)
        {
            return RegionIdToMapId.TryGetValue(regionId, out var mapId) ? mapId : null;
        }

        /// <summary>
        /// Checks if a database RegionId has a mapping to the map
        /// </summary>
        public static bool HasMapping(int regionId)
        {
            return RegionIdToMapId.ContainsKey(regionId);
        }
    }
}
