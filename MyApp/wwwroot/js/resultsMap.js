am5.ready(function () {
    var root = am5.Root.new("chartdiv");

    const COLOR_ACTIVE_DATA = am5.color(0x60a5fa);
    const COLOR_HOVER = am5.color(0x93c5fd);
    const COLOR_INACTIVE = am5.color(0xe5e7eb);
    const COLOR_ACTIVE_CLICK = am5.color(0x2a5db0);

    root.setThemes([am5themes_Animated.new(root)]);

    var chart = root.container.children.push(
        am5map.MapChart.new(root, {
            panX: "none",
            panY: "none",
            wheelX: "none",
            wheelY: "none",
            projection: am5map.geoMercator(),
            layout: root.verticalLayout
        })
    );

    var polygonSeries = chart.series.push(
        am5map.MapPolygonSeries.new(root, {
            geoJSON: am5geodata_moldovaHigh
        })
    );

    var regionDataMap = {};

    function buildRegionData() {
        if (!window.mapData || !Array.isArray(window.mapData)) return;

        regionDataMap = {};

        window.mapData.forEach(function (item) {
            var mapId = item.mapId || item.MapId;
            var regionName = item.regionName || item.RegionName;
            var regionId = item.regionId || item.RegionId;

            if (!mapId) return;

            regionDataMap[mapId] = {
                MapId: mapId,
                RegionId: regionId,
                RegionName: regionName
            };
        });

        polygonSeries.mapPolygons.each(function (polygon) {
            var dataItem = polygon.dataItem;
            if (!dataItem) return;
            var id = dataItem.get("id");

            if (regionDataMap[id]) {
                polygon.set("fill", COLOR_ACTIVE_DATA);
                polygon.set("interactive", true);
                polygon.set("tooltipText", regionDataMap[id].RegionName || "{name}");
            } else {
                polygon.set("fill", COLOR_INACTIVE);
                polygon.set("interactive", false);
                polygon.set("tooltipText", "{name}");
            }
        });
    }

    if (window.mapData && Array.isArray(window.mapData) && window.mapData.length) {
        buildRegionData();
    }

    window.addEventListener('resultsMapDataLoaded', function () {
        buildRegionData();
    });

    polygonSeries.mapPolygons.template.setAll({
        tooltipText: "{name}",
        interactive: true,
        fill: COLOR_INACTIVE,
        strokeWidth: 1,
        stroke: am5.color(0xffffff)
    });

    polygonSeries.mapPolygons.template.states.create("hover", { fill: COLOR_HOVER });
    polygonSeries.mapPolygons.template.states.create("active", { fill: COLOR_ACTIVE_CLICK });

    var selectedPolygon = null;

    function resetSelectedPolygon() {
        if (selectedPolygon) {
            selectedPolygon.set("active", false);
            selectedPolygon.set("fill", COLOR_ACTIVE_DATA);
        }
    }

    function selectRegion(dataItem, regionData) {
        resetSelectedPolygon();
        selectedPolygon = dataItem.get("mapPolygon");
        if (selectedPolygon) {
            selectedPolygon.set("active", true);
            selectedPolygon.set("fill", COLOR_ACTIVE_CLICK);
        }
        loadTopParties(regionData.RegionId, regionData.RegionName);
    }

    polygonSeries.mapPolygons.template.events.on("click", function (ev) {
        var dataItem = ev.target.dataItem;
        if (!dataItem) return;

        var regionId = dataItem.get("id");
        var regionData = regionDataMap[regionId];
        if (regionData) {
            selectRegion(dataItem, regionData);
        }
    });

    function loadTopParties(regionId, regionName) {
        var nameEl = document.getElementById('region-name');
        if (nameEl) {
            nameEl.textContent = regionName || "Se încarcă...";
        }

        var topPartiesEl = document.getElementById('top-parties-list');
        if (topPartiesEl) {
            topPartiesEl.innerHTML = '<p class="text-gray-500 text-sm text-center">Se încarcă topul partidelor...</p>';
        }

        fetch('/Rezultate/GetElectionResultsByRaion?raionId=' + regionId)
            .then(function (response) { return response.json(); })
            .then(function (data) {
                if (!topPartiesEl) return;

                if (data.success && Array.isArray(data.results) && data.results.length > 0) {
                    var html = data.results.map(function (p, index) {
                        var rank = index + 1;
                        var partyCode = p.partyCode || '';
                        var partyName = p.partyName || '';
                        var votes = p.votes || 0;

                        return '' +
                            '<div class="flex items-center justify-between px-3 py-2 rounded-lg border border-gray-100 hover:bg-gray-50 transition">' +
                            '  <div class="flex items-center gap-3 min-w-0">' +
                            '    <span class="inline-flex items-center justify-center w-6 h-6 text-xs font-bold rounded-full bg-blue-50 text-blue-700">' + rank + '</span>' +
                            '    <div class="flex flex-col min-w-0">' +
                            '      <span class="text-xs font-semibold text-gray-500">' + (partyCode || '&nbsp;') + '</span>' +
                            '      <span class="text-sm font-medium text-gray-800 truncate">' + partyName + '</span>' +
                            '    </div>' +
                            '  </div>' +
                            '  <div class="ml-3 text-sm font-semibold text-gray-900 tabular-nums">' + votes.toLocaleString("ro-RO") + '</div>' +
                            '</div>';
                    }).join('');

                    topPartiesEl.innerHTML = html;
                } else {
                    topPartiesEl.innerHTML = '<p class="text-gray-500 text-sm text-center">Nu există date pentru partide în acest raion.</p>';
                }
            })
            .catch(function (error) {
                console.error('Error loading top parties:', error);
                if (topPartiesEl) {
                    topPartiesEl.innerHTML = '<p class="text-red-500 text-sm text-center">Eroare la încărcarea partidelor.</p>';
                }
            });
    }

    chart.appear(1000, 100);
});
