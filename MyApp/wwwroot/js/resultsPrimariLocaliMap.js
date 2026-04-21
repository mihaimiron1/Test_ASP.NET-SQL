am5.ready(function () {
    var root = am5.Root.new("chartdiv");

    const COLOR_ACTIVE_DATA = am5.color(0x60a5fa);
    const COLOR_HOVER = am5.color(0x93c5fd);
    const COLOR_INACTIVE = am5.color(0xe5e7eb);
    const COLOR_ACTIVE_CLICK = am5.color(0x2a5db0);

    var ELECTION_ID = window.primariLocaliElectionId || 10064;

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
    var isLoadingLocalities = false;

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
        onRegionSelected(regionData.RegionId, regionData.RegionName, regionData.MapId);
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

    function onRegionSelected(regionId, regionName, mapId) {
        var numericId = parseInt(regionId, 10);

        var nameEl = document.getElementById('region-name');
        if (nameEl) {
            nameEl.textContent = (regionName || "Regiune") + (Number.isFinite(numericId) && numericId > 0 ? " (ID: " + numericId + ")" : "");
        }

        var idEl = document.getElementById('region-id');
        if (idEl) {
            idEl.textContent = (Number.isFinite(numericId) && numericId > 0) ? String(numericId) : "-";
        }

        var mapIdEl = document.getElementById('region-mapid');
        if (mapIdEl) {
            mapIdEl.textContent = mapId ? String(mapId) : "-";
        }

        // Show the panel, hide initial message
        var initialMsg = document.getElementById('initial-message');
        if (initialMsg) initialMsg.classList.add('hidden');

        var statsPanel = document.getElementById('region-stats');
        if (statsPanel) statsPanel.classList.remove('hidden');

        // Load localities for this region
        loadLocalities(numericId);
    }

    function loadLocalities(regionId) {
        var dropdown = document.getElementById('localities-dropdown');
        if (!dropdown) return;

        if (isLoadingLocalities) return;
        isLoadingLocalities = true;

        dropdown.onchange = null;
        dropdown.innerHTML = '<option value="">Se încarc? localit??i...</option>';
        dropdown.disabled = true;

        fetch('/Rezultate/GetVotedLocalitiesByRegion?electionId=' + ELECTION_ID + '&regionId=' + encodeURIComponent(regionId))
            .then(function (response) { return response.json(); })
            .then(function (data) {
                isLoadingLocalities = false;

                if (data.success && data.localities && data.localities.length > 0) {
                    var options = '<option value="">-- Selecteaz? localitate (' + data.localities.length + ' disponibile) --</option>';
                    data.localities.forEach(function (loc) {
                        options += '<option value="' + loc.localityId + '">' + loc.localityName + '</option>';
                    });
                    dropdown.innerHTML = options;
                    dropdown.disabled = false;

                    dropdown.onchange = function () {
                        if (this.value) {
                            var selectedText = this.options[this.selectedIndex].text;
                            console.log("Locality selected: id=", this.value, "name=", selectedText);
                            // TODO: load locality results here
                        }
                    };
                } else {
                    dropdown.innerHTML = '<option value="">F?r? localit??i disponibile</option>';
                    dropdown.disabled = true;
                }
            })
            .catch(function (error) {
                isLoadingLocalities = false;
                console.error('Error loading localities:', error);
                dropdown.innerHTML = '<option value="">Eroare la înc?rcare</option>';
                dropdown.disabled = true;
            });
    }

    chart.appear(1000, 100);
});
