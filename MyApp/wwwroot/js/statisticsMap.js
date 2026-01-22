am5.ready(function () {
    // Creare root
    var root = am5.Root.new("chartdiv");

    // Culorile definite
    const COLOR_ACTIVE_DATA = am5.color(0x60a5fa);
    const COLOR_HOVER = am5.color(0x93c5fd);
    const COLOR_INACTIVE = am5.color(0xe5e7eb);
    const COLOR_ACTIVE_CLICK = am5.color(0x2a5db0);

    // Setare tema
    root.setThemes([
        am5themes_Animated.new(root)
    ]);

    // Creare harta
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

    // Creare serie poligoane
    var polygonSeries = chart.series.push(
        am5map.MapPolygonSeries.new(root, {
            geoJSON: am5geodata_moldovaHigh
        })
    );

    // Creare dictionar pentru raioanele cu date
    var regionDataMap = {};
    var regionNameToId = {};
    var regionNames = [];

    function normalizeName(str) {
        return (str || "")
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .trim();
    }

    // Procesare date din controller
    if (window.mapData && Array.isArray(window.mapData)) {
        window.mapData.forEach(function (item) {
            var mapId = item.MapId || item.Id || item.id;
            var regionName = item.RegionName || item.Raion || item.Name || item.name;
            var regionId = item.RegionId || item.regionId;

            if (!mapId) return;

            regionDataMap[mapId] = {
                MapId: mapId,
                RegionId: regionId,
                RegionName: regionName,
                TotalVoters: item.TotalVoters || item.Votanti || 0
            };

            if (regionName) {
                var normalized = normalizeName(regionName);
                regionNameToId[normalized] = mapId;
                regionNames.push({ name: regionName, normalized: normalized, id: mapId });
            }
        });
    }

    // Configurare template pentru poligoane
    polygonSeries.mapPolygons.template.setAll({
        tooltipText: "{name}",
        interactive: true,
        fill: am5.color(0xe0e0e0),
        strokeWidth: 1,
        stroke: am5.color(0xffffff)
    });

    polygonSeries.mapPolygons.template.states.create("hover", {
        fill: COLOR_HOVER
    });

    polygonSeries.mapPolygons.template.states.create("active", {
        fill: COLOR_ACTIVE_CLICK
    });

    // Eveniment pentru fiecare poligon - colorare initiala
    polygonSeries.mapPolygons.template.events.on("dataitemchanged", function (ev) {
        var dataItem = ev.target.dataItem;
        var regionId = dataItem.get("id");

        if (regionDataMap[regionId]) {
            var polygon = ev.target;
            polygon.set("fill", COLOR_ACTIVE_DATA);
            polygon.set("interactive", true);
            polygon.set("tooltipText", regionDataMap[regionId].RegionName);
        } else {
            var polygon = ev.target;
            polygon.set("fill", COLOR_INACTIVE);
            polygon.set("interactive", false);
            polygon.set("tooltipText", "");
        }
    });

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
        // Fetch detailed statistics from server
        loadRegionStatistics(regionData.RegionId, regionData.RegionName);
    }

    // Eveniment click pe poligon
    polygonSeries.mapPolygons.template.events.on("click", function (ev) {
        var dataItem = ev.target.dataItem;
        var regionId = dataItem.get("id");
        var regionData = regionDataMap[regionId];

        if (regionData) {
            selectRegion(dataItem, regionData);
        }
    });

    // Căutare raion după nume
    var searchInput = document.getElementById("search");
    var searchButton = document.getElementById("search-button");
    var searchMessage = document.getElementById("search-message");
    var searchForm = document.getElementById("map-search-form");

    function handleSearch() {
        if (!searchInput) return;

        var rawQuery = (searchInput.value || "").trim();
        var query = normalizeName(rawQuery);
        if (!query) {
            if (searchMessage) {
                searchMessage.textContent = "";
                searchMessage.classList.add("hidden");
            }
            resetSelectedPolygon();
            return;
        }

        var mapId = regionNameToId[query];

        if (!mapId) {
            var suggestion = regionNames.find(function (r) {
                return r.normalized.startsWith(query);
            });
            if (suggestion) {
                mapId = suggestion.id;
                searchInput.value = suggestion.name;
            }
        }

        if (!mapId) {
            if (searchMessage) {
                searchMessage.textContent = "Raionul nu a fost găsit";
                searchMessage.classList.remove("hidden");
            }
            resetSelectedPolygon();
            return;
        }

        if (searchMessage) {
            searchMessage.textContent = "";
            searchMessage.classList.add("hidden");
        }

        var dataItem = polygonSeries.getDataItemById(mapId);
        var regionData = regionDataMap[mapId];
        if (dataItem && regionData) {
            selectRegion(dataItem, regionData);
        }
    }

    if (searchButton) {
        searchButton.addEventListener("click", handleSearch);
    }
    if (searchForm) {
        searchForm.addEventListener("submit", function (e) {
            e.preventDefault();
            handleSearch();
        });
    }
    if (searchInput) {
        searchInput.addEventListener("keyup", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                handleSearch();
            }
        });
    }

    // Fetch detailed region statistics from server
    function loadRegionStatistics(regionId, regionName) {
        var initialMsg = document.getElementById('initial-message');
        if (initialMsg) initialMsg.classList.add('hidden');

        var statsPanel = document.getElementById('region-stats');
        if (statsPanel) statsPanel.classList.remove('hidden');

        var nameEl = document.getElementById('region-name');
        if (nameEl) nameEl.textContent = regionName || "";

        // Show loading state
        var turnoutEl = document.getElementById('region-turnout');
        if (turnoutEl) turnoutEl.textContent = "Se încarcă...";

        var pieChartEl = document.getElementById('region-pie-chart');
        if (pieChartEl) pieChartEl.innerHTML = '<div class="flex justify-center items-center h-full"><div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div></div>';

        // Fetch from API
        fetch('/Statistics/GetRegionStatisticsForHeatMap?regionId=' + regionId)
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                if (data.success) {
                    showRegionStats(data);
                } else {
                    showError(data.message || 'Nu s-au putut încărca datele.');
                }
            })
            .catch(function (error) {
                console.error('Error loading region statistics:', error);
                showError('Eroare la încărcarea datelor.');
            });
    }

    function showError(message) {
        var turnoutEl = document.getElementById('region-turnout');
        if (turnoutEl) turnoutEl.textContent = "Eroare";

        var pieChartEl = document.getElementById('region-pie-chart');
        if (pieChartEl) {
            pieChartEl.innerHTML = '<p class="text-red-500 text-center">' + message + '</p>';
        }
    }

    // Afisare statistici detaliate
    function showRegionStats(data) {
        // Prezenta la vot
        var turnoutEl = document.getElementById('region-turnout');
        if (turnoutEl) {
            var percentage = data.votingPercentage || 0;
            turnoutEl.textContent = percentage.toFixed(1) + '%';
        }

        // Numar votanti
        var votersEl = document.getElementById('region-voters');
        if (votersEl) {
            var total = data.totalVoters || 0;
            var voted = data.votedCount || 0;
            votersEl.textContent = voted.toLocaleString("ro-RO") + " din " + total.toLocaleString("ro-RO") + " alegători";
        }

        // Creare grafic pie pentru gen
        if (document.getElementById('region-pie-chart') && data.genderStats) {
            createGenderPieChart(data.genderStats);
        }

        // Afisare statistici pe varsta (daca exista container)
        if (document.getElementById('region-age-stats') && data.ageStats) {
            renderAgeStats(data.ageStats);
        }
    }

    var pieChartInstance = null;

    function createGenderPieChart(genderStats) {
        if (pieChartInstance) {
            pieChartInstance.destroy();
        }

        if (!genderStats || genderStats.length === 0) {
            var container = document.querySelector("#region-pie-chart");
            if (container) container.innerHTML = "<p class='text-gray-500 text-center py-8'>Date indisponibile</p>";
            return;
        }

        var series = genderStats.map(function (g) { return g.voterCount; });
        var labels = genderStats.map(function (g) { return g.gender; });
        var colors = genderStats.map(function (g) { return g.color || '#3b82f6'; });

        var options = {
            series: series,
            chart: {
                type: 'pie',
                height: 350
            },
            labels: labels,
            colors: colors,
            legend: {
                position: 'bottom',
                fontSize: '14px',
                fontFamily: 'inherit'
            },
            dataLabels: {
                enabled: true,
                formatter: function (val, opts) {
                    var count = opts.w.config.series[opts.seriesIndex];
                    return count.toLocaleString('ro-RO') + ' (' + val.toFixed(1) + '%)';
                },
                style: {
                    fontSize: '12px',
                    fontWeight: 'bold',
                    colors: ['#fff']
                }
            },
            tooltip: {
                y: {
                    formatter: function (val, opts) {
                        var stats = genderStats[opts.seriesIndex];
                        return val.toLocaleString('ro-RO') + ' alegători\nAu votat: ' +
                            (stats.votedCount || 0).toLocaleString('ro-RO') +
                            ' (' + (stats.percentage || 0).toFixed(1) + '%)';
                    }
                }
            },
            responsive: [{
                breakpoint: 480,
                options: {
                    chart: { height: 300 },
                    legend: { position: 'bottom' }
                }
            }]
        };

        pieChartInstance = new ApexCharts(document.querySelector("#region-pie-chart"), options);
        pieChartInstance.render();
    }

    function renderAgeStats(ageStats) {
        var container = document.getElementById('region-age-stats');
        if (!container || !ageStats || ageStats.length === 0) return;

        var html = ageStats.map(function (item) {
            var name = item.ageCategoryName || "";
            var percentage = item.percentage || 0;
            var count = item.voterCount || 0;
            var color = item.color || "#3b82f6";

            return '<div class="flex items-center mb-2">' +
                '<div class="w-20 text-xs text-gray-600 truncate" title="' + name + '">' + name + '</div>' +
                '<div class="flex-1 mx-2">' +
                '<div class="w-full bg-gray-200 rounded-full h-4">' +
                '<div class="h-4 rounded-full flex items-center justify-end pr-1" ' +
                'style="width: ' + Math.max(percentage, 5) + '%; background-color: ' + color + ';">' +
                '<span class="text-white text-xs font-medium">' + percentage.toFixed(1) + '%</span>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div class="w-16 text-right text-xs text-gray-600">' + count.toLocaleString("ro-RO") + '</div>' +
                '</div>';
        }).join("");

        container.innerHTML = html;
    }

    polygonSeries.data.setAll([]);
    chart.appear(1000, 100);
});

// Render global age participation chart
document.addEventListener("DOMContentLoaded", function () {
    var ageChartEl = document.getElementById("age-participation-chart");
    if (!ageChartEl) return;

    var ageDataAttr = ageChartEl.getAttribute("data-age-data");
    if (!ageDataAttr) return;

    var ageData = [];
    try {
        ageData = JSON.parse(ageDataAttr);
    } catch (e) {
        console.error("Error parsing age data:", e);
        return;
    }

    if (!Array.isArray(ageData) || ageData.length === 0) return;

    var html = ageData.map(function (item) {
        var name = item.AgeCategoryName || item.Name || "";
        var percentage = item.Percentage || item.VotingPercentage || 0;
        var count = item.VoterCount || item.Count || 0;
        var color = item.Color || "#3b82f6";

        return '<div class="flex items-center mb-3">' +
            '<div class="w-24 text-sm text-gray-600 truncate" title="' + name + '">' + name + '</div>' +
            '<div class="flex-1 mx-3">' +
            '<div class="w-full bg-gray-200 rounded-full h-5">' +
            '<div class="h-5 rounded-full flex items-center justify-end pr-2" ' +
            'style="width: ' + Math.max(percentage, 3) + '%; background-color: ' + color + ';">' +
            '<span class="text-white text-xs font-medium">' + percentage.toFixed(1) + '%</span>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '<div class="w-20 text-right text-sm text-gray-600">' + count.toLocaleString("ro-RO") + '</div>' +
            '</div>';
    }).join("");

    ageChartEl.innerHTML = html;
});