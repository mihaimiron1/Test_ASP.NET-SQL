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
        const presenceValue = clampPercent(Number.isFinite(rawPresence) ? rawPresence : 0);

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



































document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('age-participation-chart');
    if (!container) return;

    const ageData = JSON.parse(container.getAttribute('data-age-data') || "[]");

    // Generează HTML
    const barsHTML = ageData.map(item => {
        const group = item.Group ?? "";
        const percentage = Number(item.Percentage) ?? 0;
        const safeWidth = Math.min(Math.max(percentage, 0), 100);
        return `
        <div class="flex items-center gap-3 sm:gap-4 mb-3 sm:mb-4">
            <div class="w-12 sm:w-16 text-xs sm:text-sm font-medium text-gray-700 text-right">
                ${group}
            </div>
            <div class="flex-1 relative">
                <div class="w-full h-8 sm:h-10 bg-gray-200 rounded-full overflow-hidden">
                    <div class="h-full rounded-full transition-all duration-1000 ease-out"
                         style="width: 0%; background-color: #2d5cf2"
                         style="width: 0%"
                         data-width="${safeWidth}%">
                    </div>
                </div>
            </div>
            <div class="w-10 sm:w-12 text-xs sm:text-sm font-semibold text-gray-800">
                ${safeWidth}%
            </div>
        </div>
        `;
    }).join('');

    container.innerHTML = barsHTML;

    // Animație
    setTimeout(() => {
        container.querySelectorAll('[data-width]').forEach(bar => {
            bar.style.width = bar.getAttribute('data-width');
        });
    }, 100);
});



























// Get the CSS variable --color-brand and convert it to hex for ApexCharts
const getBrandColor = () => {
    // Get the computed style of the document's root element
    const computedStyle = getComputedStyle(document.documentElement);

    // Get the value of the --color-brand CSS variable
    return computedStyle.getPropertyValue('--color-fg-brand').trim() || "#1447E6";
};

const getBrandSecondaryColor = () => {
    const computedStyle = getComputedStyle(document.documentElement);
    return computedStyle.getPropertyValue('--color-fg-brand-subtle').trim() || "#1447E6";
};

const getBrandTertiaryColor = () => {
    const computedStyle = getComputedStyle(document.documentElement);
    return computedStyle.getPropertyValue('--color-fg-brand-strong').trim() || "#1447E6";
};

const getNeutralPrimaryColor = () => {
    const computedStyle = getComputedStyle(document.documentElement);
    return computedStyle.getPropertyValue('--color-neutral-primary').trim() || "#1447E6";
};

const brandColor = getBrandColor();
const brandSecondaryColor = getBrandSecondaryColor();
const brandTertiaryColor = getBrandTertiaryColor();
const neutralPrimaryColor = getNeutralPrimaryColor();

const getChartOptions = () => {
    return {
        series: [52.8, 26.8, 20.4],
        colors: [brandColor, brandSecondaryColor, brandTertiaryColor],
        chart: {
            height: 420,
            width: "100%",
            type: "pie",
        },
        stroke: {
            colors: [neutralPrimaryColor],
            lineCap: "",
        },
        plotOptions: {
            pie: {
                labels: {
                    show: true,
                },
                size: "100%",
                dataLabels: {
                    offset: -25
                }
            },
        },
        labels: ["Direct", "Organic search", "Referrals"],
        dataLabels: {
            enabled: true,
            style: {
                fontFamily: "Inter, sans-serif",
            },
        },
        legend: {
            position: "bottom",
            fontFamily: "Inter, sans-serif",
        },
        yaxis: {
            labels: {
                formatter: function (value) {
                    return value + "%"
                },
            },
        },
        xaxis: {
            labels: {
                formatter: function (value) {
                    return value + "%"
                },
            },
            axisTicks: {
                show: false,
            },
            axisBorder: {
                show: false,
            },
        },
    }
}

if (document.getElementById("pie-chart") && typeof ApexCharts !== 'undefined') {
    const chart = new ApexCharts(document.getElementById("pie-chart"), getChartOptions());
    chart.render();
}
