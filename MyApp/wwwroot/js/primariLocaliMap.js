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
    var selectedPolygon = null;

    function getResultsContainer() {
        return document.getElementById("locality-results-list");
    }

    function setResultsLoading() {
        var container = getResultsContainer();
        if (!container) return;
        container.innerHTML = '<p class="text-gray-500 text-sm text-center">Se incarca top candidati...</p>';
    }

    function renderResults(items) {
        var container = getResultsContainer();
        if (!container) return;

        if (!items || !items.length) {
            container.innerHTML = '<p class="text-gray-500 text-sm text-center">Nu exista date pentru aceasta localitate.</p>';
            return;
        }

        var html = items.map(function (p) {
            var partyCode = p.partyCode || "";
            var candidateName = p.candidateName || "";
            var colorLogo = p.colorLogo || "";
            var votes = p.votes || 0;

            var logoHtml = colorLogo
                ? '<img src="' + colorLogo + '" alt="' + partyCode + '" class="w-8 h-8 rounded object-contain" />'
                : '';

            return '' +
                '<div class="flex items-center justify-between px-3 py-2 rounded-lg border border-gray-100 hover:bg-gray-50 transition">' +
                '  <div class="flex items-center gap-3 min-w-0">' +
                '    <span class="inline-flex items-center justify-center w-8 h-8">' + logoHtml + '</span>' +
                '    <div class="flex flex-col min-w-0">' +
                '      <span class="text-xs font-semibold text-gray-500">' + (partyCode || '&nbsp;') + '</span>' +
                '      <span class="text-sm font-medium text-gray-800 truncate">' + candidateName + '</span>' +
                '    </div>' +
                '  </div>' +
                '  <div class="ml-3 text-sm font-semibold text-gray-900 tabular-nums">' + votes.toLocaleString("ro-RO") + '</div>' +
                '</div>';
        }).join('');

        container.innerHTML = html;
    }

    function loadLocalityResults(localityId) {
        var numericLocalityId = parseInt(localityId, 10);
        if (!Number.isFinite(numericLocalityId) || numericLocalityId <= 0) {
            renderResults([]);
            return;
        }

        setResultsLoading();

        fetch('/Rezultate/GetElectionResultsByLocalityPrimari?localityId=' + encodeURIComponent(numericLocalityId))
            .then(function (response) { return response.json(); })
            .then(function (data) {
                if (data && data.success && Array.isArray(data.results)) {
                    renderResults(data.results);
                } else {
                    renderResults([]);
                }
            })
            .catch(function (error) {
                console.error('Error loading locality results:', error);
                renderResults([]);
            });
    }

    function buildRegionData() {
        if (!window.primariLocaliMapData || !Array.isArray(window.primariLocaliMapData)) return;

        regionDataMap = {};

        window.primariLocaliMapData.forEach(function (item) {
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

    function resetSelectedPolygon() {
        if (selectedPolygon) {
            selectedPolygon.set("active", false);
            selectedPolygon.set("fill", COLOR_ACTIVE_DATA);
        }
    }

    function onRegionSelected(regionId, regionName, mapId) {
        var numericRegionId = parseInt(regionId, 10);

        var nameEl = document.getElementById("region-name");
        if (nameEl) {
            nameEl.textContent = (regionName || "Regiune") + (Number.isFinite(numericRegionId) ? " (ID: " + numericRegionId + ")" : "");
        }

        var idEl = document.getElementById("region-id");
        if (idEl) idEl.textContent = Number.isFinite(numericRegionId) ? String(numericRegionId) : "-";

        var mapIdEl = document.getElementById("region-mapid");
        if (mapIdEl) mapIdEl.textContent = mapId ? String(mapId) : "-";

        loadLocalities(numericRegionId);
    }

    function loadLocalities(regionId) {
        var dropdown = document.getElementById("localities-dropdown");
        var msg = document.getElementById("localities-message");
        if (!dropdown) return;
        if (!Number.isFinite(regionId) || regionId <= 0) return;

        if (isLoadingLocalities) return;
        isLoadingLocalities = true;

        dropdown.disabled = true;
        dropdown.onchange = null;
        dropdown.innerHTML = '<option value="">Se incarca localitati...</option>';
        if (msg) msg.textContent = "";
        renderResults([]);

        fetch('/Rezultate/GetVotedLocalitiesByRegion?electionId=' + encodeURIComponent(ELECTION_ID) + '&regionId=' + encodeURIComponent(regionId))
            .then(function (response) { return response.json(); })
            .then(function (data) {
                isLoadingLocalities = false;

                if (data && data.success && Array.isArray(data.localities) && data.localities.length > 0) {
                    var options = '<option value="">-- Selecteaza localitate (' + data.localities.length + ' disponibile) --</option>';
                    data.localities.forEach(function (loc) {
                        options += '<option value="' + loc.localityId + '">' + loc.localityName + '</option>';
                    });
                    dropdown.innerHTML = options;
                    dropdown.disabled = false;
                    if (msg) msg.textContent = "Selecteaza localitatea pentru top 10 candidati.";

                    dropdown.onchange = function () {
                        var localityId = this.value;
                        if (!localityId) {
                            renderResults([]);
                            return;
                        }
                        loadLocalityResults(localityId);
                    };
                } else {
                    dropdown.innerHTML = '<option value="">Fara localitati disponibile</option>';
                    dropdown.disabled = true;
                    if (msg) msg.textContent = "Nu exista localitati pentru regiunea selectata.";
                }
            })
            .catch(function (error) {
                isLoadingLocalities = false;
                console.error("Error loading localities:", error);
                dropdown.innerHTML = '<option value="">Eroare la incarcare</option>';
                dropdown.disabled = true;
                if (msg) msg.textContent = "A aparut o eroare la incarcarea localitatilor.";
            });
    }

    polygonSeries.mapPolygons.template.setAll({
        tooltipText: "{name}",
        interactive: true,
        fill: COLOR_INACTIVE,
        strokeWidth: 1,
        stroke: am5.color(0xffffff)
    });

    polygonSeries.mapPolygons.template.states.create("hover", { fill: COLOR_HOVER });
    polygonSeries.mapPolygons.template.states.create("active", { fill: COLOR_ACTIVE_CLICK });

    polygonSeries.mapPolygons.template.events.on("click", function (ev) {
        var dataItem = ev.target.dataItem;
        if (!dataItem) return;

        var mapId = dataItem.get("id");
        var regionData = regionDataMap[mapId];
        if (!regionData) return;

        resetSelectedPolygon();
        selectedPolygon = dataItem.get("mapPolygon");
        if (selectedPolygon) {
            selectedPolygon.set("active", true);
            selectedPolygon.set("fill", COLOR_ACTIVE_CLICK);
        }

        onRegionSelected(regionData.RegionId, regionData.RegionName, regionData.MapId);
    });

    if (window.primariLocaliMapData && Array.isArray(window.primariLocaliMapData) && window.primariLocaliMapData.length) {
        buildRegionData();
    }

    window.addEventListener("primariLocaliMapDataLoaded", function () {
        buildRegionData();
    });

    chart.appear(1000, 100);
});
