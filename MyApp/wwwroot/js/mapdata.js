var simplemaps_countrymap_mapdata={
  main_settings: {
   //General settings
    width: "responsive", //'700' or 'responsive'
    background_color: "#FFFFFF",
    background_transparent: "yes",
    border_color: "#ffffff",
    
    //State defaults
    state_description: "State description",
    state_color: "#cccccd",
    state_hover_color: "#3B729F",
    state_url: "",
    border_size: 1.5,
    all_states_inactive: "no",
    all_states_zoomable: "yes",
    
    //Location defaults
    location_description: "Location description",
    location_url: "",
    location_color: "#FF0067",
    location_opacity: 0.8,
    location_hover_opacity: 1,
    location_size: 25,
    location_type: "square",
    location_image_source: "frog.png",
    location_border_color: "#FFFFFF",
    location_border: 2,
    location_hover_border: 2.5,
    all_locations_inactive: "no",
    all_locations_hidden: "no",
    
    //Label defaults
    label_color: "#ffffff",
    label_hover_color: "#ffffff",
    label_size: 16,
    label_font: "Arial",
    label_display: "auto",
    label_scale: "yes",
    hide_labels: "no",
    hide_eastern_labels: "no",
   
    //Zoom settings
    zoom: "yes",
    manual_zoom: "yes",
    back_image: "no",
    initial_back: "no",
    initial_zoom: "-1",
    initial_zoom_solo: "no",
    region_opacity: 1,
    region_hover_opacity: 0.6,
    zoom_out_incrementally: "yes",
    zoom_percentage: 0.99,
    zoom_time: 0.5,
    
    //Popup settings
    popup_color: "white",
    popup_opacity: 0.9,
    popup_shadow: 1,
    popup_corners: 5,
    popup_font: "12px/1.5 Verdana, Arial, Helvetica, sans-serif",
    popup_nocss: "no",
    
    //Advanced settings
    div: "map",
    auto_load: "yes",
    url_new_tab: "no",
    images_directory: "default",
    fade_time: 0.1,
    link_text: "View Website",
    popups: "detect",
    state_image_url: "",
    state_image_position: "",
    location_image_url: ""
  },
  state_specific: {
    MDAN: {
      name: "Anenii Noi"
    },
    MDBA: {
      name: "Balti"
    },
    MDBD: {
      name: "Bender"
    },
    MDBR: {
      name: "Briceni"
    },
    MDBS: {
      name: "Basarabeasca"
    },
    MDCA: {
      name: "Cahul"
    },
    MDCL: {
      name: "Calarasi"
    },
    MDCM: {
      name: "Cimislia"
    },
    MDCR: {
      name: "Criuleni"
    },
    MDCS: {
      name: "Causeni"
    },
    MDCT: {
      name: "Cantemir"
    },
    MDCU: {
      name: "Chisinau"
    },
    MDDO: {
      name: "Donduseni"
    },
    MDDR: {
      name: "Drochia"
    },
    MDDU: {
      name: "Dubasari"
    },
    MDED: {
      name: "Edinet"
    },
    MDFA: {
      name: "Falesti"
    },
    MDFL: {
      name: "Floresti"
    },
    MDGA: {
      name: "Unitate Teritoriala Autonoma Gagauzia"
    },
    MDGL: {
      name: "Glodeni"
    },
    MDHI: {
      name: "Hincesti"
    },
    MDIA: {
      name: "Ialoveni"
    },
    MDLE: {
      name: "Leova"
    },
    MDNI: {
      name: "Nisporeni"
    },
    MDOC: {
      name: "Ocnita"
    },
    MDOR: {
      name: "Orhei"
    },
    MDRE: {
      name: "Rezina"
    },
    MDRI: {
      name: "Riscani"
    },
    MDSD: {
      name: "Soldanesti"
    },
    MDSI: {
      name: "Singerei"
    },
    MDSN: {
      name: "Transnistria"
    },
    MDSO: {
      name: "Soroca"
    },
    MDST: {
      name: "Straseni"
    },
    MDSV: {
      name: "Stefan Voda"
    },
    MDTA: {
      name: "Taraclia"
    },
    MDTE: {
      name: "Telenesti"
    },
    MDUN: {
      name: "Ungheni"
    }
  },
  locations: {},
  labels: {
    MDAN: {
      name: "Anenii Noi",
      parent_id: "MDAN"
    },
    MDBA: {
      name: "Balti",
      parent_id: "MDBA"
    },
    MDBD: {
      name: "Bender",
      parent_id: "MDBD"
    },
    MDBR: {
      name: "Briceni",
      parent_id: "MDBR"
    },
    MDBS: {
      name: "Basarabeasca",
      parent_id: "MDBS"
    },
    MDCA: {
      name: "Cahul",
      parent_id: "MDCA"
    },
    MDCL: {
      name: "Calarasi",
      parent_id: "MDCL"
    },
    MDCM: {
      name: "Cimislia",
      parent_id: "MDCM"
    },
    MDCR: {
      name: "Criuleni",
      parent_id: "MDCR"
    },
    MDCS: {
      name: "Causeni",
      parent_id: "MDCS"
    },
    MDCT: {
      name: "Cantemir",
      parent_id: "MDCT"
    },
    MDCU: {
      name: "Chisinau",
      parent_id: "MDCU"
    },
    MDDO: {
      name: "Donduseni",
      parent_id: "MDDO"
    },
    MDDR: {
      name: "Drochia",
      parent_id: "MDDR"
    },
    MDDU: {
      name: "Dubasari",
      parent_id: "MDDU"
    },
    MDED: {
      name: "Edinet",
      parent_id: "MDED"
    },
    MDFA: {
      name: "Falesti",
      parent_id: "MDFA"
    },
    MDFL: {
      name: "Floresti",
      parent_id: "MDFL"
    },
    MDGA: {
      name: "Unitate Teritoriala Autonoma Gagauzia",
      parent_id: "MDGA"
    },
    MDGL: {
      name: "Glodeni",
      parent_id: "MDGL"
    },
    MDHI: {
      name: "Hincesti",
      parent_id: "MDHI"
    },
    MDIA: {
      name: "Ialoveni",
      parent_id: "MDIA"
    },
    MDLE: {
      name: "Leova",
      parent_id: "MDLE"
    },
    MDNI: {
      name: "Nisporeni",
      parent_id: "MDNI"
    },
    MDOC: {
      name: "Ocnita",
      parent_id: "MDOC"
    },
    MDOR: {
      name: "Orhei",
      parent_id: "MDOR"
    },
    MDRE: {
      name: "Rezina",
      parent_id: "MDRE"
    },
    MDRI: {
      name: "Riscani",
      parent_id: "MDRI"
    },
    MDSD: {
      name: "Soldanesti",
      parent_id: "MDSD"
    },
    MDSI: {
      name: "Singerei",
      parent_id: "MDSI"
    },
    MDSN: {
      name: "Transnistria",
      parent_id: "MDSN"
    },
    MDSO: {
      name: "Soroca",
      parent_id: "MDSO"
    },
    MDST: {
      name: "Straseni",
      parent_id: "MDST"
    },
    MDSV: {
      name: "Stefan Voda",
      parent_id: "MDSV"
    },
    MDTA: {
      name: "Taraclia",
      parent_id: "MDTA"
    },
    MDTE: {
      name: "Telenesti",
      parent_id: "MDTE"
    },
    MDUN: {
      name: "Ungheni",
      parent_id: "MDUN"
    }
  },
  legend: {
    entries: []
  },
  regions: {}
};