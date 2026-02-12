am5.ready(function () {
    console.log("=== Heat Map Initialization ===");
    
    const data = Array.isArray(window.mapData) ? window.mapData : [];
    console.log("✅ Map data loaded:", data.length, "regions");
    console.log("📊 Raw data:", data);

    // Build lookup by MapId (GeoJSON ID like MD-AN, MD-BS, etc.)
    const dataByMapId = {};
    data.forEach(d => {
        if (d.MapId) {
            dataByMapId[d.MapId] = d;
            console.log("✅ Mapped region:", d.MapId, "→", d.RegionName, "| RegionId:", d.RegionId);
        } else {
            console.warn("⚠️ Region without MapId:", d);
        }
    });

    console.log("📍 Total regions mapped:", Object.keys(dataByMapId).length);

    // Root and theme
    var root = am5.Root.new("chartdiv");
    root.setThemes([am5themes_Animated.new(root)]);

    // Chart
    var chart = root.container.children.push(am5map.MapChart.new(root, {
        panX: "none",
        panY: "none",
        wheelX: "none",
        wheelY: "none",
        projection: am5map.geoMercator(),
        layout: root.verticalLayout
    }));

    // Polygon series
    var polygonSeries = chart.series.push(am5map.MapPolygonSeries.new(root, {
        geoJSON: am5geodata_moldovaHigh
    }));

    // Colors
    const COLOR_VOTED = am5.color(0x3b82f6);      // Blue for regions with votes
    const COLOR_HOVER = am5.color(0x60a5fa);      // Lighter blue for hover
    const COLOR_SELECTED = am5.color(0x1d4ed8);   // Darker blue for selected
    const COLOR_NO_DATA = am5.color(0xffffff);    // WHITE for regions without data

    // Set defaults - MAKE SURE INTERACTIVE IS TRUE
    polygonSeries.mapPolygons.template.setAll({
        tooltipText: "{name}",
        interactive: true,  // Enable by default
        stroke: am5.color(0x94a3b8),
        strokeWidth: 1.5,
        cursorOverStyle: "pointer"
    });

    // Prepare data array for polygons
    var geoFeatures = am5geodata_moldovaHigh.features;
    console.log("🗺️ Total GeoJSON features:", geoFeatures.length);

    var polygonData = geoFeatures.map(f => {
        const mapId = f.id;
        const name = f.properties && f.properties.name || "";
        const info = dataByMapId[mapId];

        const hasData = !!info;
        const fillColor = hasData ? COLOR_VOTED : COLOR_NO_DATA;

        if (hasData) {
            console.log("🟦 Region WITH data:", mapId, name, "| RegionId:", info.RegionId, "| Voters:", info.TotalVoters);
        }

        return {
            id: mapId,
            name: name,
            hasData: hasData,
            regionId: hasData ? info.RegionId : null,
            regionName: hasData ? info.RegionName : name,
            totalVoters: hasData ? info.TotalVoters : null,
            tooltip: hasData
                ? `${name}\nVotanți: ${info.TotalVoters.toLocaleString('ro-RO')}\n(Click pentru detalii)`
                : `${name}\n(Nu au votat)`,
            fill: fillColor,
            // IMPORTANT: All regions should be interactive, but only ones with data do something
            interactive: true
        };
    });

    polygonSeries.data.setAll(polygonData);

    // Apply fill color
    polygonSeries.mapPolygons.template.adapters.add("fill", function (fill, target) {
        if (!target.dataItem) return fill;
        var d = target.dataItem.dataContext;
        return d && d.fill ? d.fill : COLOR_NO_DATA;
    });

    // Apply tooltip
    polygonSeries.mapPolygons.template.adapters.add("tooltipText", function (text, target) {
        if (!target.dataItem) return text;
        var d = target.dataItem.dataContext;
        return d && d.tooltip ? d.tooltip : text;
    });

    // Hover effect
    polygonSeries.mapPolygons.template.states.create("hover", {
        fill: COLOR_HOVER
    });

    // Selected state
    polygonSeries.mapPolygons.template.states.create("active", {
        fill: COLOR_SELECTED
    });

    // Track selected polygon
    var selectedPolygon = null;

    // Click event handler
    polygonSeries.mapPolygons.template.events.on("click", function (ev) {
        console.log("🖱️ CLICK EVENT TRIGGERED!");
        
        var dataItem = ev.target.dataItem;
        if (!dataItem) {
            console.error("❌ No dataItem on click");
            return;
        }

        var dataContext = dataItem.dataContext;
        console.log("📍 Clicked region data:", dataContext);

        if (!dataContext || !dataContext.hasData) {
            console.warn("⚠️ Clicked region has no data - showing alert");
            alert("Această regiune nu are date de vot.");
            return;
        }

        console.log("✅ Loading statistics for:", dataContext.regionName, "| ID:", dataContext.regionId);

        // Reset previous selection
        if (selectedPolygon && selectedPolygon !== ev.target) {
            selectedPolygon.states.apply("default");
        }

        // Set new selection
        selectedPolygon = ev.target;
        selectedPolygon.states.apply("active");

        // Load region statistics
        loadRegionStatistics(dataContext.regionId, dataContext.regionName);
    });

    console.log("✅ Click handler attached to polygons");

    chart.appear(1000, 100);

    // ============================================================
    // Load and display region statistics
    // ============================================================
    var genderChartInstance = null;
    var ageChartInstance = null;

    function loadRegionStatistics(regionId, regionName) {
        console.log("📊 Loading statistics for region:", regionId, regionName);

        // Show loading state
        const statsPanel = document.getElementById('region-stats');
        const initialMessage = document.getElementById('initial-message');
        
        if (initialMessage) {
            console.log("✅ Hiding initial message");
            initialMessage.classList.add('hidden');
        }
        if (statsPanel) {
            console.log("✅ Showing stats panel");
            statsPanel.classList.remove('hidden');
        }

        // Show loading indicator
        const regionNameEl = document.getElementById('region-name');
        if (regionNameEl) {
            regionNameEl.textContent = `${regionName} - Se încarcă...`;
        }

        // Fetch statistics from API
        const apiUrl = `/Statistics/GetRegionStatisticsForHeatMap?regionId=${regionId}`;
        console.log("🌐 Fetching from:", apiUrl);

        fetch(apiUrl)
            .then(response => {
                console.log("📡 Response status:", response.status);
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("📦 API Response:", data);

                if (!data.success) {
                    console.error("❌ API returned error:", data.message);
                    alert(data.message || "Eroare la încărcarea statisticilor");
                    return;
                }

                console.log("✅ Statistics loaded successfully");

                // Update region name
                if (regionNameEl) {
                    regionNameEl.textContent = data.regionName;
                }

                // Update total voters
                const totalVotersEl = document.getElementById('region-total-voters');
                if (totalVotersEl) {
                    totalVotersEl.textContent = data.totalVoters.toLocaleString('ro-RO') + ' votanți';
                }

                // Render charts
                console.log("📊 Rendering gender chart...");
                renderGenderChart(data.genderStats);
                
                console.log("📊 Rendering age chart...");
                renderAgeChart(data.ageStats);
                
                console.log("✅ All charts rendered");
            })
            .catch(error => {
                console.error("❌ Error fetching statistics:", error);
                alert("Eroare la încărcarea statisticilor: " + error.message);
            });
    }

    // ============================================================
    // Render Gender Pie Chart
    // ============================================================
    function renderGenderChart(genderStats) {
        console.log("🎨 Rendering gender chart with data:", genderStats);

        // Destroy previous chart
        if (genderChartInstance) {
            genderChartInstance.destroy();
        }

        const chartEl = document.querySelector("#gender-chart");
        if (!chartEl) {
            console.error("❌ Gender chart element not found!");
            return;
        }

        if (!genderStats || genderStats.length === 0) {
            console.warn("⚠️ No gender statistics to display");
            chartEl.innerHTML = '<p class="text-gray-500 text-center">Nu sunt date disponibile</p>';
            return;
        }

        const labels = genderStats.map(g => g.gender);
        const series = genderStats.map(g => g.votedCount);
        const colors = genderStats.map(g => g.color);

        var options = {
            series: series,
            chart: {
                type: 'pie',
                height: 300,
                animations: {
                    enabled: false
                }
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
                        return val.toLocaleString('ro-RO') + ' votanți';
                    }
                }
            }
        };

        genderChartInstance = new ApexCharts(chartEl, options);
        genderChartInstance.render();

        // Display gender details
        const detailsEl = document.getElementById('gender-details');
        if (detailsEl) {
            detailsEl.innerHTML = genderStats.map(g => `
                <div class="flex items-center justify-between p-2 bg-gray-50 rounded">
                    <div class="flex items-center gap-2">
                        <div class="w-3 h-3 rounded-full" style="background-color: ${g.color}"></div>
                        <span class="text-sm font-medium">${g.gender}</span>
                    </div>
                    <div class="text-right">
                        <div class="text-sm font-semibold">${g.votedCount.toLocaleString('ro-RO')}</div>
                        <div class="text-xs text-gray-500">${g.percentage.toFixed(1)}%</div>
                    </div>
                </div>
            `).join('');
        }

        console.log("✅ Gender chart rendered");
    }

    // ============================================================
    // Render Age Bar Chart
    // ============================================================
    function renderAgeChart(ageStats) {
        console.log("🎨 Rendering age chart with data:", ageStats);

        // Destroy previous chart
        if (ageChartInstance) {
            ageChartInstance.destroy();
        }

        const chartEl = document.querySelector("#age-chart");
        if (!chartEl) {
            console.error("❌ Age chart element not found!");
            return;
        }

        if (!ageStats || ageStats.length === 0) {
            console.warn("⚠️ No age statistics to display");
            chartEl.innerHTML = '<p class="text-gray-500 text-center">Nu sunt date disponibile</p>';
            return;
        }

        const categories = ageStats.map(a => a.ageCategoryName);
        const series = ageStats.map(a => a.voterCount);
        const colors = ageStats.map(a => a.color);

        var options = {
            series: [{
                name: 'Votanți',
                data: series
            }],
            chart: {
                type: 'bar',
                height: 350
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    distributed: true,
                    dataLabels: {
                        position: 'top'
                    }
                }
            },
            colors: colors,
            dataLabels: {
                enabled: true,
                formatter: function (val) {
                    return val.toLocaleString('ro-RO');
                },
                offsetY: -20,
                style: {
                    fontSize: '12px',
                    colors: ["#304758"]
                }
            },
            xaxis: {
                categories: categories,
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            yaxis: {
                title: {
                    text: 'Număr votanți'
                },
                labels: {
                    formatter: function (val) {
                        return val.toLocaleString('ro-RO');
                    }
                }
            },
            legend: {
                show: false
            },
            tooltip: {
                y: {
                    formatter: function (val, opts) {
                        const percentage = ageStats[opts.dataPointIndex].percentage;
                        return `${val.toLocaleString('ro-RO')} votanți (${percentage.toFixed(1)}%)`;
                    }
                }
            }
        };

        ageChartInstance = new ApexCharts(chartEl, options);
        ageChartInstance.render();

        console.log("✅ Age chart rendered");
    }

    console.log("=== Heat Map Initialization Complete ===");
});
