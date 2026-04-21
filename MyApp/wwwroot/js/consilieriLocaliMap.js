am5.ready(function () {
    var root = am5.Root.new("chartdiv");

    const COLOR_ACTIVE_DATA = am5.color(0x60a5fa);
    const COLOR_HOVER = am5.color(0x93c5fd);
    const COLOR_INACTIVE = am5.color(0xe5e7eb);
    const COLOR_ACTIVE_CLICK = am5.color(0x2a5db0);

    var ELECTION_ID = window.consilieriLocaliElectionId || 10066;

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

    function renderResults(items) {
        var container = document.getElementById("locality-results-list");
        if (!container) return;

        container.innerHTML = "";

        if (!items || !items.length) {
            container.innerHTML = '<p class="text-gray-500 text-sm text-center">Nu există date pentru această localitate.</p>';
            return;
        }

        items.forEach(function (p) {
            var partyCode = p.partyCode || "";
            var candidateName = p.candidateName || "";
            var colorLogo = (typeof p.colorLogo === "string" ? p.colorLogo.trim() : "");
            var votes = p.votes || 0;

            var row = document.createElement("div");
            row.className = "flex items-center justify-between px-3 py-2 rounded-lg border border-gray-100 hover:bg-gray-50 transition";

            var left = document.createElement("div");
            left.className = "flex items-center gap-3 min-w-0";

            var logoWrap = document.createElement("span");
            logoWrap.className = "inline-flex items-center justify-center w-8 h-8";

            if (colorLogo) {
                var img = document.createElement("img");
                img.src = colorLogo;
                img.alt = partyCode;
                img.className = "w-8 h-8 rounded object-contain";
                img.loading = "lazy";
                img.onerror = function () {
                    this.remove();
                };
                logoWrap.appendChild(img);
            }

            var textWrap = document.createElement("div");
            textWrap.className = "flex flex-col min-w-0";

            var code = document.createElement("span");
            code.className = "text-xs font-semibold text-gray-500";
            code.textContent = partyCode || "\u00A0";

            var cand = document.createElement("span");
            cand.className = "text-sm font-medium text-gray-800 truncate";
            cand.textContent = candidateName;

            textWrap.appendChild(code);
            textWrap.appendChild(cand);

            left.appendChild(logoWrap);
            left.appendChild(textWrap);

            var right = document.createElement("div");
            right.className = "ml-3 text-sm font-semibold text-gray-900 tabular-nums";
            right.textContent = votes.toLocaleString("ro-RO");

            row.appendChild(left);
            row.appendChild(right);
            container.appendChild(row);
        });
    }

    function setResultsLoading() {
        var container = document.getElementById("locality-results-list");
        if (!container) return;
        container.innerHTML = '<p class="text-gray-500 text-sm text-center">Se încarcă top candidați...</p>';
    }

    function loadLocalityResults(localityId) {
        var numericLocalityId = parseInt(localityId, 10);
        if (!Number.isFinite(numericLocalityId) || numericLocalityId <= 0) {
            renderResults([]);
            return;
        }

        setResultsLoading();

        fetch('/Rezultate/GetElectionResultsByLocalityConsilieri?localityId=' + encodeURIComponent(numericLocalityId))
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
        if (!window.consilieriLocaliMapData || !Array.isArray(window.consilieriLocaliMapData)) return;

        regionDataMap = {};

        window.consilieriLocaliMapData.forEach(function (item) {
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
        dropdown.innerHTML = '<option value="">Se încarcă localități...</option>';
        if (msg) msg.textContent = "";
        renderResults([]);

        fetch('/Rezultate/GetVotedLocalitiesByRegion?electionId=' + encodeURIComponent(ELECTION_ID) + '&regionId=' + encodeURIComponent(regionId))
            .then(function (response) { return response.json(); })
            .then(function (data) {
                isLoadingLocalities = false;

                if (data && data.success && Array.isArray(data.localities) && data.localities.length > 0) {
                    var options = '<option value="">-- Selectează localitate (' + data.localities.length + ' disponibile) --</option>';
                    data.localities.forEach(function (loc) {
                        options += '<option value="' + loc.localityId + '">' + loc.localityName + '</option>';
                    });
                    dropdown.innerHTML = options;
                    dropdown.disabled = false;
                    if (msg) msg.textContent = "Selectează localitatea pentru top 10 candidați.";

                    dropdown.onchange = function () {
                        var localityId = this.value;
                        if (!localityId) {
                            renderResults([]);
                            return;
                        }
                        loadLocalityResults(localityId);
                    };
                } else {
                    dropdown.innerHTML = '<option value="">Fără localități disponibile</option>';
                    dropdown.disabled = true;
                    if (msg) msg.textContent = "Nu există localități pentru regiunea selectată.";
                }
            })
            .catch(function (error) {
                isLoadingLocalities = false;
                console.error("Error loading localities:", error);
                dropdown.innerHTML = '<option value="">Eroare la încărcare</option>';
                dropdown.disabled = true;
                if (msg) msg.textContent = "A apărut o eroare la încărcarea localităților.";
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

    if (window.consilieriLocaliMapData && Array.isArray(window.consilieriLocaliMapData) && window.consilieriLocaliMapData.length) {
        buildRegionData();
    }

    window.addEventListener("consilieriLocaliMapDataLoaded", function () {
        buildRegionData();
    });

    chart.appear(1000, 100);
});
