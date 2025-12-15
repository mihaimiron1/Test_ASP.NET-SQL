
am5.ready(function () {
    // Creare root
    var root = am5.Root.new("chartdiv");


    // Culorile definite
    const COLOR_ACTIVE_DATA = am5.color(0x60a5fa); // Albastru deschis (culoarea inițială a raioanelor cu date)
    const COLOR_HOVER = am5.color(0x93c5fd);       // Albastru foarte deschis (hover)
    const COLOR_INACTIVE = am5.color(0xe5e7eb);
    const COLOR_ACTIVE_CLICK = am5.color(0x2a5db0); // Albastru inchis la click

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
            geoJSON: am5geodata_moldovaHigh,
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
            .replace(/[\u0300-\u036f]/g, "") // elimina diacritice
            .trim();
    }
    if (window.mapData) {
        window.mapData.forEach(function (item) {
            regionDataMap[item.Id] = item;
            if (item.Raion) {
                var normalized = normalizeName(item.Raion);
                regionNameToId[normalized] = item.Id;
                regionNames.push({ name: item.Raion, normalized, id: item.Id });
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

    // Configurare states pentru hover si seletie
    polygonSeries.mapPolygons.template.states.create("hover", {
        fill: COLOR_HOVER // Albastru deschis pentru hover
    });

    polygonSeries.mapPolygons.template.states.create("active", {
        fill: COLOR_ACTIVE_CLICK // Albastru pentru selectie
    });

    // Eveniment pentru fiecare poligon
    polygonSeries.mapPolygons.template.events.on("dataitemchanged", function (ev) {
        var dataItem = ev.target.dataItem;
        var regionId = dataItem.get("id");

        if (regionDataMap[regionId]) {
            // Acest raion are date - il facem activ
            var polygon = ev.target;
            polygon.set("fill", COLOR_ACTIVE_DATA); // Albastru pentru raioane cu date
            polygon.set("interactive", true);
            polygon.set("tooltipText", regionDataMap[regionId].Raion);
        } else {
            // Acest raion nu are date - il dezactivam
            var polygon = ev.target;
            polygon.set("fill", COLOR_INACTIVE); // Gri pentru raioane fara date
            polygon.set("interactive", false);
            polygon.set("tooltipText", "");
        }
    });

    // Retine ultimul poligon selectat pentru a-l reseta la culoarea standard
    var selectedPolygon = null;

    // Retine ultimul poligon selectat pentru a-l reseta la culoarea standard
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
        showRegionStats(regionData);
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

    // Căutare raion după nume din controller
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

        var regionId = regionNameToId[query];

        // Daca nu exista match exact, incearca autocomplete pe inceput de cuvant
        if (!regionId) {
            var suggestion = regionNames.find(function (r) {
                return r.normalized.startsWith(query);
            });
            if (suggestion) {
                regionId = suggestion.id;
                searchInput.value = suggestion.name; // autocomplete vizual
            }
        }

        if (!regionId) {
            if (searchMessage) {
                searchMessage.textContent = "Raionul nu participa in scrutin";
                searchMessage.classList.remove("hidden");
            }
            resetSelectedPolygon();
            return;
        }

        if (searchMessage) {
            searchMessage.textContent = "";
            searchMessage.classList.add("hidden");
        }

        var dataItem = polygonSeries.getDataItemById(regionId);
        var regionData = regionDataMap[regionId];
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

    // Functie pentru afisarea statisticilor raionului
    function showRegionStats(data) {
        // Ascunde mesajul initial
        var initialMsg = document.getElementById('initial-message');
        if (initialMsg) initialMsg.classList.add('hidden');

        // Afiseaza panoul cu statistici
        var statsPanel = document.getElementById('region-stats');
        if (statsPanel) statsPanel.classList.remove('hidden');

        // Actualizeaza numele raionului
        var nameEl = document.getElementById('region-name');
        if (nameEl) nameEl.textContent = data.Raion || "";

        // Actualizeaza prezența la vot
        var turnoutEl = document.getElementById('region-turnout');
        if (turnoutEl) {
            turnoutEl.textContent = (data.ProcenteVot || 0) + '%';
        }

        var votersEl = document.getElementById('region-voters');
        if (votersEl) {
            votersEl.textContent = data.Votanti ? data.Votanti.toLocaleString("ro-RO") + " votanți" : "";
        }

        // Creare/actualizare grafic pie (daca exista containerul)
        if (document.getElementById('region-pie-chart')) {
            createPieChart(data);
        }

        // Afiseaza lista de candidati (daca exista)
        renderCandidates(data);
    }

    // Variabila pentru a stoca instanta graficului
    var pieChartInstance = null;

    // Functie pentru crearea graficului pie
    function createPieChart(data) {
        // Distruge graficul existent daca exista
        if (pieChartInstance) {
            pieChartInstance.destroy();
        }

        // Daca lipsesc datele de gen, nu afisam graficul
        if (data.ProcenteFemei === undefined || data.ProcenteBarbati === undefined) {
            var container = document.querySelector("#region-pie-chart");
            if (container) container.innerHTML = "";
            return;
        }

        var options = {
            series: [data.ProcenteFemei, data.ProcenteBarbati],
            chart: {
                type: 'pie',
                height: 400
            },
            labels: ['Femei', 'Bărbați'],
            colors: ['#f87171', '#3b82f6'],
            legend: {
                position: 'bottom',
                fontSize: '14px',
                fontFamily: 'inherit'
            },
            dataLabels: {
                enabled: true,
                formatter: function (val) {
                    return val.toFixed(1) + '%';
                },
                style: {
                    fontSize: '14px',
                    fontWeight: 'bold',
                    colors: ['#fff']
                }
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val.toFixed(1) + '%';
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

    function renderCandidates(data) {
        var listEl = document.getElementById('candidate-list');
        if (!listEl) return;

        listEl.innerHTML = "";

        if (!data.Candidates || !Array.isArray(data.Candidates) || data.Candidates.length === 0) {
            listEl.innerHTML = '<p class="text-sm text-gray-500">Nu sunt date pentru candidați.</p>';
            return;
        }

        data.Candidates.forEach(function (cand) {
            var votes = (cand.ProcenteVot && data.Votanti)
                ? Math.round((cand.ProcenteVot / 100) * data.Votanti)
                : null;

            var card = document.createElement('div');
            card.className = "flex items-center gap-4 border border-gray-300 rounded-lg p-4 shadow-sm";

            var logo = document.createElement('img');
            logo.src = cand.PartidLogo || "/images/default.png";
            logo.alt = cand.Partid || "Logo";
            logo.className = "w-16 h-16 object-contain border border-gray-300 rounded";

            var content = document.createElement('div');
            content.className = "flex-1";

            var title = document.createElement('p');
            title.className = "text-base font-semibold text-gray-800";
            var votesText = votes !== null ? " - " + votes.toLocaleString("ro-RO") + " votanți" : "";
            title.textContent = (cand.Partid || "Partid") + " - " + (cand.ProcenteVot || 0) + "% " + votesText;

            var sub = document.createElement('p');
            sub.className = "text-sm text-gray-700";
            sub.textContent = cand.Candidat || "";

            content.appendChild(title);
            content.appendChild(sub);

            card.appendChild(logo);
            card.appendChild(content);

            listEl.appendChild(card);
        });
    }

    // Setare date pentru serie
    polygonSeries.data.setAll([]);

    // Animatie initiala
    chart.appear(1000, 100);
});