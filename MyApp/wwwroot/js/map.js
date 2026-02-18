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
            .replace(/[\u0300-\u036f]/g, "")
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

    // Configurare states pentru hover si selectie
    polygonSeries.mapPolygons.template.states.create("hover", {
        fill: COLOR_HOVER
    });

    polygonSeries.mapPolygons.template.states.create("active", {
        fill: COLOR_ACTIVE_CLICK
    });

    // Eveniment pentru fiecare poligon
    polygonSeries.mapPolygons.template.events.on("dataitemchanged", function (ev) {
        var dataItem = ev.target.dataItem;
        var regionId = dataItem.get("id");

        if (regionDataMap[regionId]) {
            var polygon = ev.target;
            polygon.set("fill", COLOR_ACTIVE_DATA);
            polygon.set("interactive", true);
            polygon.set("tooltipText", regionDataMap[regionId].Raion);
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

        if (!regionId) {
            var suggestion = regionNames.find(function (r) {
                return r.normalized.startsWith(query);
            });
            if (suggestion) {
                regionId = suggestion.id;
                searchInput.value = suggestion.name;
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

        // Afiseaza numarul de votanti si total alegatori
        var votersEl = document.getElementById('region-voters');
        if (votersEl) {
            var votanti = data.Votanti || 0;
            var totalAlegatori = data.TotalAlegatori || data.Alegatori || 0;
            if (totalAlegatori > 0) {
                votersEl.innerHTML = '<span class="font-medium text-gray-700">' + votanti.toLocaleString("ro-RO") + '</span> voturi din <span class="font-medium text-gray-700">' + totalAlegatori.toLocaleString("ro-RO") + '</span> alegători';
            } else {
                votersEl.textContent = votanti ? votanti.toLocaleString("ro-RO") + " votanți" : "";
            }
        }

        // Creare/actualizare grafic pie (daca exista containerul)
        if (document.getElementById('region-pie-chart')) {
            createPieChart(data);
        }

        // Afiseaza statisticile pe categorii de varsta
        if (document.getElementById('region-age-stats')) {
            renderAgeStats(data);
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
            if (container) container.innerHTML = "<p class='text-gray-500 text-center py-4'>Date indisponibile</p>";
            return;
        }

        // Calculeaza numarul de votanti pe gen
        var totalVotanti = data.Votanti || 0;
        var numarFemei = data.NumarFemei || Math.round(totalVotanti * (data.ProcenteFemei / 100));
        var numarBarbati = data.NumarBarbati || Math.round(totalVotanti * (data.ProcenteBarbati / 100));

        var options = {
            series: [data.ProcenteFemei, data.ProcenteBarbati],
            chart: {
                type: 'pie',
                height: 280,
                animations: { enabled: false }
            },
            labels: ['Femei', 'Bărbați'],
            colors: ['#f87171', '#3b82f6'],
            legend: {
                position: 'bottom',
                fontSize: '13px',
                fontFamily: 'inherit',
                formatter: function (seriesName, opts) {
                    var count = seriesName === 'Femei' ? numarFemei : numarBarbati;
                    return seriesName + ': ' + count.toLocaleString('ro-RO');
                }
            },
            dataLabels: {
                enabled: true,
                formatter: function (val, opts) {
                    var count = opts.seriesIndex === 0 ? numarFemei : numarBarbati;
                    return val.toFixed(1) + '%\n(' + count.toLocaleString('ro-RO') + ')';
                },
                style: {
                    fontSize: '11px',
                    fontWeight: 'bold',
                    colors: ['#fff']
                }
            },
            tooltip: {
                y: {
                    formatter: function (val, opts) {
                        var count = opts.seriesIndex === 0 ? numarFemei : numarBarbati;
                        return val.toFixed(1) + '% (' + count.toLocaleString('ro-RO') + ' votanți)';
                    }
                }
            },
            responsive: [{
                breakpoint: 480,
                options: {
                    chart: { height: 250 },
                    legend: { position: 'bottom' }
                }
            }]
        };

        pieChartInstance = new ApexCharts(document.querySelector("#region-pie-chart"), options);
        pieChartInstance.render();
    }

    // Functie pentru afisarea statisticilor pe categorii de varsta
    function renderAgeStats(data) {
        var container = document.getElementById('region-age-stats');
        if (!container) return;

        // Verifica daca exista date pentru categorii de varsta
        var ageData = data.CategoriiVarsta || data.AgeCategories || data.ageStats || [];

        if (!ageData || ageData.length === 0) {
            container.innerHTML = '<p class="text-gray-500 text-center py-4">Date indisponibile pentru categorii de vârstă</p>';
            return;
        }

        var html = ageData.map(function (item) {
            // Suport pentru diferite formate de date
            var name = item.Categorie || item.AgeCategoryName || item.Name || item.name || "";
            var percentage = item.Procent || item.Percentage || item.percentage || 0;
            var count = item.Numar || item.VoterCount || item.Count || item.count || 0;
            var votedCount = item.NumarVotat || item.VotedCount || item.votedCount || 0;
            var color = item.Color || item.color || "#3b82f6";

            // Calculeaza procentul daca nu exista
            if (percentage === 0 && count > 0 && votedCount > 0) {
                percentage = (votedCount / count * 100);
            }

            return '<div class="flex items-center gap-3 py-2 px-3 rounded-lg hover:bg-gray-50">' +
                '<div class="w-3 h-3 rounded-full flex-shrink-0" style="background-color: ' + color + ';"></div>' +
                '<div class="flex-1 min-w-0">' +
                '<div class="flex items-center justify-between mb-1">' +
                '<span class="text-sm font-medium text-gray-700 truncate">' + name + '</span>' +
                '<span class="text-sm font-semibold ml-2" style="color: ' + color + ';">' + percentage.toFixed(1) + '%</span>' +
                '</div>' +
                '<div class="w-full bg-gray-200 rounded-full h-1.5">' +
                '<div class="h-1.5 rounded-full" style="width: ' + Math.max(percentage, 2) + '%; background-color: ' + color + ';"></div>' +
                '</div>' +
                '<div class="text-xs text-gray-500 mt-1">' +
                (votedCount > 0 ? votedCount.toLocaleString("ro-RO") + ' din ' : '') +
                count.toLocaleString("ro-RO") + ' alegători' +
                '</div>' +
                '</div>' +
                '</div>';
        }).join("");

        container.innerHTML = html;
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