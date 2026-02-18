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
    var regionNameToId = {};
    var regionNames = [];

    // Flag-uri pentru a preveni apeluri simultane / recursive
    var isLoadingRegion = false;
    var isLoadingLocality = false;
    var isLoadingLocalities = false;

    function normalizeName(str) {
        return (str || "").toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").trim();
    }

    // Procesează datele (refactor pentru a suporta încărcare asincronă)
    function processMapData() {
        if (!window.mapData || !Array.isArray(window.mapData)) return;

        // resetează colecțiile în caz că funcția este apelată de mai multe ori
        regionDataMap = {};
        regionNameToId = {};
        regionNames = [];

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

            if (regionName) {
                var normalized = normalizeName(regionName);
                regionNameToId[normalized] = mapId;
                regionNames.push({ name: regionName, normalized: normalized, id: mapId });
            }
        });

        // Trigger updates for polygons that already exist so fill states are applied
        try {
            Object.keys(regionDataMap).forEach(function (mapId) {
                var dataItem = polygonSeries.getDataItemById(mapId);
                if (dataItem) {
                    dataItem.set("data", regionDataMap[mapId]);
                    dataItem.set("value", regionDataMap[mapId].RegionId || 1);
                }
            });
        } catch (e) {
            console.warn('processMapData update error:', e);
        }
    }

    // Dacă datele sunt deja încărcate, procesează imediat
    if (window.mapData && Array.isArray(window.mapData) && window.mapData.length) processMapData();

    // Ascultă evenimentul setat din HeatMap.cshtml când fetch-ul s-a terminat
    window.addEventListener('mapDataLoaded', function () { processMapData(); });

    polygonSeries.mapPolygons.template.setAll({
        tooltipText: "{name}",
        interactive: true,
        fill: COLOR_INACTIVE,
        strokeWidth: 1,
        stroke: am5.color(0xffffff)
    });

    polygonSeries.mapPolygons.template.states.create("hover", { fill: COLOR_HOVER });
    polygonSeries.mapPolygons.template.states.create("active", { fill: COLOR_ACTIVE_CLICK });

    polygonSeries.mapPolygons.template.events.on("dataitemchanged", function (ev) {
        var dataItem = ev.target.dataItem;
        var regionId = dataItem.get("id");
        var polygon = ev.target;

        if (regionDataMap[regionId]) {
            polygon.set("fill", COLOR_ACTIVE_DATA);
            polygon.set("interactive", true);
            polygon.set("tooltipText", regionDataMap[regionId].RegionName);
        } else {
            polygon.set("fill", COLOR_INACTIVE);
            polygon.set("interactive", false);
        }
    });

    var selectedPolygon = null;
    var currentRegionId = null;
    var currentRegionName = null;

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
        loadRegionStatistics(regionData.RegionId, regionData.RegionName);
    }

    polygonSeries.mapPolygons.template.events.on("click", function (ev) {
        var dataItem = ev.target.dataItem;
        var regionId = dataItem.get("id");
        var regionData = regionDataMap[regionId];
        if (regionData) selectRegion(dataItem, regionData);
    });

    // Search
    var searchInput = document.getElementById("search");
    var searchButton = document.getElementById("search-button");
    var searchMessage = document.getElementById("search-message");
    var searchForm = document.getElementById("map-search-form");

    function handleSearch() {
        if (!searchInput) return;
        var rawQuery = (searchInput.value || "").trim();
        var query = normalizeName(rawQuery);

        if (!query) {
            if (searchMessage) { searchMessage.textContent = ""; searchMessage.classList.add("hidden"); }
            resetSelectedPolygon();
            return;
        }

        var mapId = regionNameToId[query];
        if (!mapId) {
            var suggestion = regionNames.find(function (r) { return r.normalized.startsWith(query); });
            if (suggestion) { mapId = suggestion.id; searchInput.value = suggestion.name; }
        }

        if (!mapId) {
            if (searchMessage) { searchMessage.textContent = "Raionul nu a fost găsit"; searchMessage.classList.remove("hidden"); }
            resetSelectedPolygon();
            return;
        }

        if (searchMessage) { searchMessage.textContent = ""; searchMessage.classList.add("hidden"); }

        var dataItem = polygonSeries.getDataItemById(mapId);
        var regionData = regionDataMap[mapId];
        if (dataItem && regionData) selectRegion(dataItem, regionData);
    }

    if (searchButton) searchButton.addEventListener("click", handleSearch);
    if (searchForm) searchForm.addEventListener("submit", function (e) { e.preventDefault(); handleSearch(); });
    if (searchInput) searchInput.addEventListener("keyup", function (e) { if (e.key === "Enter") { e.preventDefault(); handleSearch(); } });

    // ─────────────────────────────────────────────────────────────
    // Setează loading UI (spinner) pentru grafice
    // ─────────────────────────────────────────────────────────────
    function setChartsLoading() {
        var pieChartEl = document.getElementById('region-pie-chart');
        if (pieChartEl) pieChartEl.innerHTML = '<div class="flex justify-center items-center h-full"><div class="w-10 h-10 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div></div>';

        var ageStatsEl = document.getElementById('region-age-stats');
        if (ageStatsEl) ageStatsEl.innerHTML = '<div class="text-center py-4"><div class="inline-block w-6 h-6 border-2 border-gray-400 border-t-transparent rounded-full animate-spin"></div></div>';
    }

    // ─────────────────────────────────────────────────────────────
    // Încarcă statisticile unui raion
    // ─────────────────────────────────────────────────────────────
    function loadRegionStatistics(regionId, regionName) {
        // Previne apeluri simultane
        if (isLoadingRegion) return;
        isLoadingRegion = true;

        currentRegionId = regionId;
        currentRegionName = regionName;

        var initialMsg = document.getElementById('initial-message');
        if (initialMsg) initialMsg.classList.add('hidden');

        var statsPanel = document.getElementById('region-stats');
        if (statsPanel) statsPanel.classList.remove('hidden');

        var ageSection = document.getElementById('age-stats-section');
        if (ageSection) ageSection.classList.remove('hidden');

        var nameEl = document.getElementById('region-name');
        if (nameEl) nameEl.textContent = regionName || "Se încarcă...";

        setChartsLoading();

        // Încarcă localitățile independent (fără să blocheze graficele)
        loadLocalities(regionId);

        fetch('/Statistics/GetRegionStatisticsForHeatMap?regionId=' + regionId)
            .then(function (response) { return response.json(); })
            .then(function (data) {
                isLoadingRegion = false;
                if (data.success) {
                    showRegionStats(data);
                } else {
                    showError(data.message || 'Eroare la încărcare');
                }
            })
            .catch(function (error) {
                isLoadingRegion = false;
                console.error('Error loading region stats:', error);
                showError('Eroare de conexiune');
            });
    }

    // ─────────────────────────────────────────────────────────────
    // Reîncarcă statisticile raionului curent (fără a reîncărca localitățile)
    // ─────────────────────────────────────────────────────────────
    function reloadRegionStatistics(regionId, regionName) {
        // Previne apeluri simultane
        if (isLoadingRegion) return;
        isLoadingRegion = true;

        var nameEl = document.getElementById('region-name');
        if (nameEl) nameEl.textContent = regionName || "Se încarcă...";

        setChartsLoading();

        fetch('/Statistics/GetRegionStatisticsForHeatMap?regionId=' + regionId)
            .then(function (response) { return response.json(); })
            .then(function (data) {
                isLoadingRegion = false;
                if (data.success) {
                    showRegionStats(data);
                } else {
                    showError(data.message || 'Eroare la încărcare');
                }
            })
            .catch(function (error) {
                isLoadingRegion = false;
                console.error('Error reloading region stats:', error);
                showError('Eroare de conexiune');
            });
    }

    // ─────────────────────────────────────────────────────────────
    // Încarcă localitățile dintr-un raion în dropdown
    // ─────────────────────────────────────────────────────────────
    function loadLocalities(regionId) {
        var localitiesDropdown = document.getElementById('localities-dropdown');
        if (!localitiesDropdown) {
            console.error('Localities dropdown not found!');
            return;
        }

        // Previne apeluri simultane
        if (isLoadingLocalities) return;
        isLoadingLocalities = true;

        // Elimină event listener-ul vechi înainte de a seta unul nou
        localitiesDropdown.onchange = null;

        localitiesDropdown.innerHTML = '<option value="">Se încarcă localități...</option>';
        localitiesDropdown.disabled = true;

        fetch('/Statistics/GetLocalitiesByRegion?regionId=' + regionId)
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                isLoadingLocalities = false;

                if (data.success && data.localities && data.localities.length > 0) {
                    var options = '<option value="">-- Selectează localitate (' + data.localities.length + ' disponibile) --</option>';
                    data.localities.forEach(function (loc) {
                        var typeName = loc.regionTypeName || '';
                        var displayName = loc.name + (typeName ? ' (' + typeName + ')' : '');
                        options += '<option value="' + loc.regionId + '">' + displayName + '</option>';
                    });
                    localitiesDropdown.innerHTML = options;
                    localitiesDropdown.disabled = false;

                    // Setează handler-ul O SINGURĂ DATĂ după ce dropdown-ul e populat
                    localitiesDropdown.onchange = function () {
                        if (this.value) {
                            var selectedText = this.options[this.selectedIndex].text;
                            loadLocalityStatistics(parseInt(this.value), selectedText);
                        } else {
                            // Revino la statisticile raionului fără a reîncărca localitățile
                            if (currentRegionId && currentRegionName) {
                                reloadRegionStatistics(currentRegionId, currentRegionName);
                            }
                        }
                    };
                } else {
                    localitiesDropdown.innerHTML = '<option value="">Fără localități disponibile</option>';
                    localitiesDropdown.disabled = true;
                }
            })
            .catch(function (error) {
                isLoadingLocalities = false;
                console.error('Error loading localities:', error);
                localitiesDropdown.innerHTML = '<option value="">Eroare la încărcare</option>';
                localitiesDropdown.disabled = true;
            });
    }

    // ─────────────────────────────────────────────────────────────
    // Încarcă statisticile unei localități
    // ─────────────────────────────────────────────────────────────
    function loadLocalityStatistics(localityId, localityName) {
        // Previne apeluri simultane
        if (isLoadingLocality) return;
        isLoadingLocality = true;

        var nameEl = document.getElementById('region-name');
        if (nameEl) nameEl.textContent = localityName || "Se încarcă...";

        setChartsLoading();

        fetch('/Statistics/GetLocalityStatistics?regionId=' + localityId)
            .then(function (response) { return response.json(); })
            .then(function (data) {
                isLoadingLocality = false;
                if (data.success) {
                    showRegionStats(data);
                } else {
                    showError(data.message || 'Eroare la încărcare');
                }
            })
            .catch(function (error) {
                isLoadingLocality = false;
                console.error('Error loading locality stats:', error);
                showError('Eroare de conexiune');
            });
    }

    // ─────────────────────────────────────────────────────────────
    // Afișează eroare în panoul de statistici
    // ─────────────────────────────────────────────────────────────
    function showError(message) {
        var pieChartEl = document.getElementById('region-pie-chart');
        if (pieChartEl) pieChartEl.innerHTML = '<p class="text-red-500 text-center py-8">' + message + '</p>';
        var ageStatsEl = document.getElementById('region-age-stats');
        if (ageStatsEl) ageStatsEl.innerHTML = '';
    }

    // ─────────────────────────────────────────────────────────────
    // Afișează statisticile în UI
    // ─────────────────────────────────────────────────────────────
    function showRegionStats(data) {
        if (document.getElementById('region-pie-chart') && data.genderStats) {
            createPieChart(data.genderStats);
        }
        if (document.getElementById('region-age-stats') && data.ageStats) {
            renderAgeStats(data.ageStats);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Pie Chart (ApexCharts)
    // ─────────────────────────────────────────────────────────────
    var pieChartInstance = null;

    function createPieChart(genderStats) {
        // Distruge instanța veche corect
        if (pieChartInstance) {
            try { pieChartInstance.destroy(); } catch (e) { console.warn('Chart destroy error:', e); }
            pieChartInstance = null;
        }

        var container = document.querySelector("#region-pie-chart");
        if (!container) return;

        // Resetează containerul înainte de a crea un chart nou
        container.innerHTML = "";

        if (!genderStats || genderStats.length === 0) {
            container.innerHTML = "<p class='text-gray-500 text-center py-8'>Nu există date</p>";
            return;
        }

        var series = genderStats.map(function (g) { return g.voterCount || 0; });
        var labels = genderStats.map(function (g) { return g.gender || 'Necunoscut'; });
        var colors = genderStats.map(function (g) { return g.color || '#3b82f6'; });

        var totalVotes = series.reduce(function (a, b) { return a + b; }, 0);
        if (totalVotes === 0) {
            container.innerHTML = "<p class='text-gray-500 text-center py-8'>Nu există voturi</p>";
            return;
        }

        var options = {
            series: series,
            chart: {
                type: 'pie',
                height: 280,
                animations: { enabled: false }
            },
            labels: labels,
            colors: colors,
            legend: {
                position: 'bottom',
                fontSize: '14px',
                formatter: function (seriesName, opts) {
                    var count = opts.w.config.series[opts.seriesIndex];
                    return seriesName + ': ' + count.toLocaleString('ro-RO');
                }
            },
            dataLabels: {
                enabled: true,
                formatter: function (val, opts) {
                    var count = opts.w.config.series[opts.seriesIndex];
                    return val.toFixed(1) + '%\n(' + count.toLocaleString('ro-RO') + ')';
                },
                style: { fontSize: '12px', fontWeight: 'bold', colors: ['#fff'] }
            }
        };

        pieChartInstance = new ApexCharts(container, options);
        pieChartInstance.render();
    }

    // ─────────────────────────────────────────────────────────────
    // Statistici vârstă (bare progres)
    // ─────────────────────────────────────────────────────────────
    function renderAgeStats(ageStats) {
        var container = document.getElementById('region-age-stats');
        if (!container) return;

        if (!ageStats || ageStats.length === 0) {
            container.innerHTML = '<p class="text-gray-500 text-center py-4">Nu există date</p>';
            return;
        }

        var html = ageStats.map(function (item) {
            var name = item.ageCategoryName || "";
            var percentage = item.percentage || 0;
            var voterCount = item.voterCount || 0;
            var color = item.color || "#3b82f6";

            return '<div class="flex items-center gap-3 p-3 rounded-lg hover:bg-gray-50 transition">' +
                '<div class="w-3 h-3 rounded-full flex-shrink-0" style="background-color: ' + color + ';"></div>' +
                '<div class="flex-1 min-w-0">' +
                '<div class="flex items-center justify-between mb-1.5">' +
                '<span class="text-sm font-semibold text-gray-800">' + name + '</span>' +
                '<span class="text-sm font-bold" style="color: ' + color + ';">' + percentage.toFixed(1) + '%</span>' +
                '</div>' +
                '<div class="w-full bg-gray-200 rounded-full h-2">' +
                '<div class="h-2 rounded-full transition-all duration-300" style="width: ' + Math.max(percentage, 2) + '%; background-color: ' + color + ';"></div>' +
                '</div>' +
                '<div class="text-xs text-gray-600 mt-1">' + voterCount.toLocaleString("ro-RO") + ' voturi</div>' +
                '</div>' +
                '</div>';
        }).join("");

        container.innerHTML = html;
    }

    polygonSeries.data.setAll([]);
    chart.appear(1000, 100);
});

// ─────────────────────────────────────────────────────────────
// Chart vârstă din atribut data (pagina principală)
// ─────────────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", function () {
    var ageChartEl = document.getElementById("age-participation-chart");
    if (!ageChartEl) return;

    var ageDataAttr = ageChartEl.getAttribute("data-age-data");
    if (!ageDataAttr) {
        ageChartEl.innerHTML = '<p class="text-gray-500 text-center py-8">Selectați un raion pentru a vedea statisticile pe vârstă</p>';
        return;
    }

    var ageData = [];
    try { ageData = JSON.parse(ageDataAttr); } catch (e) {
        console.error('Error parsing age data:', e);
        return;
    }

    if (!Array.isArray(ageData) || ageData.length === 0) {
        ageChartEl.innerHTML = '<p class="text-gray-500 text-center py-8">Nu există date pentru categorii de vârstă</p>';
        return;
    }

    var html = ageData.map(function (item) {
        var name = item.ageCategoryName || item.AgeCategoryName || item.Name || "";
        var percentage = item.percentage || item.Percentage || 0;
        var voterCount = item.voterCount || item.VoterCount || 0;
        var color = item.color || item.Color || "#3b82f6";

        return '<div class="mb-6 group">' +
            '<div class="flex justify-between items-center mb-2">' +
            '<div class="flex items-center gap-2">' +
            '<div class="w-3 h-3 rounded-full shadow-sm" style="background-color: ' + color + ';"></div>' +
            '<span class="text-sm font-semibold text-gray-800">' + name + '</span>' +
            '</div>' +
            '<span class="text-sm font-bold tabular-nums" style="color: ' + color + ';">' + percentage.toFixed(1) + '%</span>' +
            '</div>' +
            '<div class="w-full bg-gray-200 rounded-full h-6 overflow-hidden shadow-inner">' +
            '<div class="h-6 rounded-full flex items-center px-2 text-xs font-semibold text-white transition-all duration-700 ease-out" ' +
            'style="width: ' + Math.max(percentage, 1) + '%; background: linear-gradient(90deg, ' + color + ' 0%, ' + color + 'dd 100%);">' +
            (percentage > 10 ? voterCount.toLocaleString("ro-RO") : '') +
            '</div>' +
            '</div>' +
            '<div class="text-xs text-gray-500 mt-1.5 font-medium">' +
            '<span class="text-gray-700">' + voterCount.toLocaleString("ro-RO") + '</span> voturi din total' +
            '</div>' +
            '</div>';
    }).join("");

    ageChartEl.innerHTML = html;
});