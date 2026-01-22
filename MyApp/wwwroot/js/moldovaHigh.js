// Moldova GeoJSON for Highcharts
// This file wraps the am5 data and makes it available as moldovaHigh

// Load the am5 data
if (typeof window.am5geodata_moldovaHigh !== 'undefined') {
    var moldovaHigh = window.am5geodata_moldovaHigh;
    console.log('✅ moldovaHigh loaded from am5geodata_moldovaHigh');
} else {
    console.error('❌ am5geodata_moldovaHigh not found!');
}

// Ensure hc-key property exists for all features
if (typeof moldovaHigh !== 'undefined' && moldovaHigh.features) {
    moldovaHigh.features.forEach(function(feature) {
        if (feature.properties && feature.properties.id && !feature.properties['hc-key']) {
            feature.properties['hc-key'] = feature.properties.id.toLowerCase();
        }
    });
}