document.addEventListener('DOMContentLoaded', () => {
    // Primary brand color used for the chart segment
    const getBrandColor = () => {
        const computedStyle = getComputedStyle(document.documentElement);
        return computedStyle.getPropertyValue('--color-fg-brand').trim() || "#2d5cf2";
    };

    const getMutedColor = () => {
        // Culoare fixă pentru "Persoane care nu au votat"
        return "#d2d4db";
    };

    // Clamp helper to keep values between 0 and 100
    const clampPercent = (value) => Math.min(Math.max(value, 0), 100);

    const brandColor = getBrandColor();
    const mutedColor = getMutedColor();
    const constantTextColor = "#111827"; // Culoare constantă pentru textul din centru

    const setInnerLabelColor = (host, color) => {
        if (!host) return;
        const selectors = [
            ".apexcharts-datalabel-label",
            ".apexcharts-datalabel-value",
            ".apexcharts-datalabel-total-label",
            ".apexcharts-datalabel-total-value"
        ];
        host.querySelectorAll(selectors.join(",")).forEach(el => {
            el.style.setProperty("fill", color, "important");
            el.style.setProperty("color", color, "important");
        });
    };

    const getChartOptions = (presenceValue) => ({
        series: [presenceValue, 100 - presenceValue],
        labels: ["Prezența la vot", "Persoane care nu au votat"],
        colors: [brandColor, mutedColor],
        chart: {
            height: 320,
            width: "100%",
            type: "donut",
            events: {
                dataPointSelection: (event, chartContext, config) => {
                    const label = config.w.globals.labels[config.dataPointIndex] || "";
                    const value = config.w.globals.series[config.dataPointIndex];

                    // Actualizează textul din centru cu valoarea selectată
                    const hostEl = chartContext.el;
                    if (hostEl?.parentElement) {
                        const totalLabel = hostEl.parentElement.querySelector(".apexcharts-datalabel-label");
                        const totalValue = hostEl.parentElement.querySelector(".apexcharts-datalabel-value");
                        if (totalLabel) totalLabel.textContent = label;
                        if (totalValue) totalValue.textContent = `${value.toFixed(1)}%`;
                    }

                    // Menține culoarea constantă
                    setInnerLabelColor(hostEl?.parentElement, constantTextColor);
                },
                mounted: (chartContext) => {
                    // Setează culoarea constantă la montare
                    const hostEl = chartContext.el;
                    setInnerLabelColor(hostEl?.parentElement, constantTextColor);
                }
            }
        },
        stroke: {
            colors: ["transparent"],
            width: 0
        },
        plotOptions: {
            pie: {
                donut: {
                    size: "80%",
                    labels: {
                        show: true,
                        total: {
                            show: true,
                            showAlways: true,
                            label: "Prezența la vot",
                            formatter: () => `${presenceValue.toFixed(1)}%`,
                            color: constantTextColor,
                            fontSize: "16px",
                            fontWeight: 600
                        },
                        value: {
                            show: true,
                            formatter: (value) => `${value.toFixed(1)}%`,
                            color: constantTextColor,
                            fontSize: "24px",
                            fontWeight: 700,
                            offsetY: 16
                        },
                    },
                },
            },
        },
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: false
        },
        tooltip: {
            enabled: false
        }
    });

    const chartHost = document.getElementById("donut-chart");
    if (chartHost && typeof ApexCharts !== 'undefined') {
        const rawPresence = Number(chartHost.dataset.presence);
        const presenceValue = clampPercent(Number.isFinite(rawPresence) ? rawPresence : 63);

        // Prevent duplicate renders (e.g., partial reloads) by destroying any existing chart
        if (chartHost.__apexInstance) {
            chartHost.__apexInstance.destroy();
            chartHost.__apexInstance = null;
        } else {
            chartHost.innerHTML = "";
        }

        const chart = new ApexCharts(chartHost, getChartOptions(presenceValue));
        chartHost.__apexInstance = chart;
        chart.render().then(() => {
            // Setează culoarea constantă după render
            setInnerLabelColor(chartHost, constantTextColor);
        });
    }

    // Ascunde elementul de selecție dacă există (textul de jos)
    const selectionLabel = document.getElementById("chart-selection-label");
    if (selectionLabel) {
        selectionLabel.style.display = "none";
    }
});



































