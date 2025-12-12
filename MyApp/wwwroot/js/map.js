am5.ready(function () {

    var root = am5.Root.new("chartdiv");

    root.setThemes([
        am5themes_Animated.new(root)
    ]);

    var chart = root.container.children.push(
        am5map.MapChart.new(root, {
            projection: am5map.geoMercator()
        })
    );

    var polygonSeries = chart.series.push(
        am5map.MapPolygonSeries.new(root, {
            geoJSON: am5geodata_moldovaHigh,
            valueField: "value" // va fi folosit mai tarziu pentru heat
        })
    );

    // Stil default (gri)
    polygonSeries.mapPolygons.template.setAll({
        fill: am5.color(0xdddddd),
        stroke: am5.color(0xffffff),
        strokeWidth: 1,
        interactive: true,
        tooltipText: "{name}"
    });

    polygonSeries.mapPolygons.template.set("tooltipPosition", "fixed");
    polygonSeries.mapPolygons.template.set("tooltipX", am5.percent(50));
    polygonSeries.mapPolygons.template.set("tooltipY", am5.percent(0));


    polygonSeries.set("tooltip", am5.Tooltip.new(root, {
        pointerOrientation: "vertical",
        autoTextColor: false
    }));

    polygonSeries.get("tooltip").get("background").setAll({
        fill: am5.color(0x6b7f2c),
        stroke: am5.color(0xffffff),
        cornerRadius: 6,
        fillOpacity: 1
    });

    polygonSeries.get("tooltip").label.setAll({
        fill: am5.color(0xffffff),
        fontSize: 14,
        fontWeight: "500",
        paddingTop: 6,
        paddingBottom: 6,
        paddingLeft: 10,
        paddingRight: 10
    });

    // Hover
    polygonSeries.mapPolygons.template.states.create("hover", {
        fill: am5.color(0x9aa86a)
    });


});
