//Date.prototype.getWeek = function () {
//    var onejan = new Date(this.getFullYear(),0,1);
//    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay()+1)/7);
//}
//Date.prototype.addDays = function (days) {
//    var dat = new Date(this.valueOf());
//    dat.setDate(dat.getDate() + days);
//    return dat;
//}

var viewModel = {
    materiaalList: [],
    aantalList: [],
    daysOfWeek: [],
    startDatum: null,
    eindDatum: null,
    session : window.sessionStorage,
    selectedWeek : null,
    init: function () {
        //Nagaan of het op dit moment weekend is. Zoja, dan worden de dagen van de volgende week geblokkeerd.
        var weekend = IsWeekend();
        var vrijdagNaVijf = VrijdagNaVijf();
        var dagen;
        if (weekend || vrijdagNaVijf) {
            dagen = viewModel.getDaysOfNextWeek();
            viewModel.daysOfWeek = $.map(dagen, function(date) {
                var dag = date.getDate();
                var maand = date.getMonth() + 1;
                var jaar = date.getFullYear();
                var datum = dag + '/' + maand + '/' + jaar;
                return datum;
            });
        } else {
            dagen = viewModel.getDaysOfWeek();
            viewModel.daysOfWeek = $.map(dagen, function (date) {
                var dag = date.getDate();
                var maand = date.getMonth() + 1;
                var jaar = date.getFullYear();
                var datum = dag + '/' + maand + '/' + jaar;
                return datum;
            });
        }

        $("#verlanglijst-pagina .checkbox").change(function () {
            
            //Selected row
            var materiaalId = $(this).find("input")[0].id;
            var materiaalRij = $("#" + materiaalId);
            if ($(this).children().is(":checked")) {
                // highligt selected row    
                materiaalRij.css("background-color", "#dff0d8");   
            } else {
                // dehighligt selected row
                materiaalRij.css('background', 'transparent');
            }
 
            //Wanneer een van de twee checkboxen aangeklikt wordt, krijgt de tweede dezelfde waarde als de eerste
            var box = $("#verlanglijst-pagina .checkbox").find("." + materiaalId);
            var selectedBox = $(this).find("input")[0].checked;
            box[0].checked = selectedBox;
            box[1].checked = selectedBox;
        });
        $("#reservatie-date").datepicker({
            changeMonth: true,
            changeYear: true,
            startDate: '+1d',
            daysOfWeekDisabled: [0,6],
            format: "dd/mm/yyyy",
            language: "nl",
            todayHighlight: true,
            calenderWeeks: true,
            beforeShowDay: function (date) {
                var dag = date.getDate();
                var maand = date.getMonth() + 1;
                var jaar = date.getFullYear();
                var datum = dag + '/' + maand + '/' + jaar;

                if (viewModel.daysOfWeek.indexOf(datum) > -1) return false;
            }
        }).on('changeDate', function (ev) {
            $(this).blur();
            $(this).datepicker('hide');
            viewModel.selectedWeek = ev.date;
            viewModel.materiaalList = [];
            viewModel.aantalList = [];
            $('input:checkbox:checked').map(function () {
                var materiaalId = $(this).parent().find("input")[0].id;
                var aantal = $("#" + materiaalId).find($(".input-medium")).val();
                if (viewModel.materiaalList.indexOf(parseInt(materiaalId)) < 0) {
                    viewModel.materiaalList.push(parseInt(materiaalId));
                    viewModel.aantalList.push(parseInt(aantal));
                }
            });
            var selectedWeek = viewModel.getWeek(Date.parse($("input[name='date']")[0].value));
            var startDatum = $("input[name='date']")[0].value;
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Verlanglijst/Controle",
                data: { materiaal: viewModel.materiaalList, aantal: viewModel.aantalList, knop : false, startDatum: startDatum },
                success: function (data) {
                    $("#verlanglijst-pagina").html(data);
                    viewModel.init();
                }
            });
        });
        $("#reservatie-end-date").daterangepicker({
            "showDropdowns": true,
            "locale": {
                "format": "DD/MM/YYYY",       
                "separator": " - ",
                "applyLabel": "Kies",
                "cancelLabel": "Annuleer",
                "fromLabel": "From",
                "toLabel": "To",
                "customRangeLabel": "Custom",
                "daysOfWeek": [
                    "Zo",
                    "Ma",
                    "Di",
                    "Wo",
                    "Do",
                    "Vr",
                    "Za"
                ],
                "monthNames": [
                    "Januari",
                    "Februari",
                    "Maart",
                    "April",
                    "Mei",
                    "Juni",
                    "July",
                    "Augustus",
                    "September",
                    "October",
                    "November",
                    "December"
                ],
                "firstDay": 1
            },
            "alwaysShowCalendars": true,
            //"startDate": Date.parse("tomorrow").toLocaleDateString(),
            //"endDate": Date.parse("tomorrow").toLocaleDateString(),
            "minDate": Date.parse("today").toLocaleDateString()
        }, function(start, end, label) {
            
        }).on('apply.daterangepicker', function (ev, picker) {
            var startDatum = picker.startDate.format('DD-MM-YYYY');
            var eindDatum = picker.endDate.format('DD-MM-YYYY');
            var datums = $("#reservatie-end-date").val();
            var delen = datums.split("-");
            viewModel.startDatum = delen[0];
            viewModel.eindDatum = delen[1];
            $('input:checkbox:checked').map(function () {
                var materiaalId = $(this).parent().find("input")[0].id;
                var aantal = $("#" + materiaalId).find($(".input-medium")).val();
                if (viewModel.materiaalList.indexOf(parseInt(materiaalId)) < 0) {
                    viewModel.materiaalList.push(parseInt(materiaalId));
                    viewModel.aantalList.push(parseInt(aantal));
                }
            });
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Verlanglijst/Controle",
                data: { materiaal: viewModel.materiaalList, aantal: viewModel.aantalList, startDatum: viewModel.startDatum, eindDatum : viewModel.eindDatum, knop: false },
                success: function (data) {
                    $("#verlanglijst-pagina").html(data);
                    viewModel.init();
                }
            });
        });
        $(".detail-materiaal").click(function () {
            var materiaalId = $(this).parent().parent().find("input")[0].id;
            $.get("/Verlanglijst/ReservatieDetails", { id: materiaalId, week: -1 }, function (data) {
                $("#verlanglijst-pagina").html(data);
                $.get("/Verlanglijst/ReservatieDetailsGrafiek", { id: materiaalId }, function(dataMateriaal) {
                    google.charts.load('current', { packages: ['corechart', 'bar'] });
                    google.charts.setOnLoadCallback(function() {
                        drawMaterial(dataMateriaal);
                    });
                    
                });
                viewModel.init();
            });
        });
        $("#reservatie-detail-date").datepicker({
            changeMonth: true,
            changeYear: true,
            startDate: '+1d',
            daysOfWeekDisabled: [0, 6],
            format: "dd/mm/yyyy",
            language: "nl",
            todayHighlight: true,
            calenderWeeks: true
        }).on('changeDate', function (ev) {
            $(this).blur();
            $(this).datepicker('hide');
            var date = Date.parse($("input[name='date']")[0].value);
            var selectedWeek = viewModel.getWeek(date);
            var materiaalId = parseInt($(".materiaal-naam")[0].id);
            $.get("/Verlanglijst/ReservatieDetails", { id: materiaalId, week: selectedWeek }, function (data) {
                $("#verlanglijst-pagina").html(data);
                viewModel.init();
            });
        });
        $("#btn-confirmeer").click(function () {
            var invalid;
            var selectedWeek;
            if (typeof  $("input[name='date']")[0] !== "undefined") {
                var date = Date.parse($("input[name='date']")[0].value);
                selectedWeek = viewModel.getWeek(date);
                viewModel.selectedWeek = selectedWeek;
                viewModel.startDatum = $("input[name='date']")[0].value;
            } else {
                var datums = $("#reservatie-end-date").val();
                var delen = datums.split("-");
                viewModel.startDatum = delen[0];
                viewModel.eindDatum = delen[1];
                viewModel.selectedWeek = viewModel.startDatum + "-" + viewModel.eindDatum;
            }
            
           
            if (viewModel.selectedWeek !== null) {
                if ($('input:checkbox:checked').length === 0) {
                    $(".foutmelding").text("Selecteer minstens 1 materiaal!");
                    return false;
                }
                viewModel.materiaalList = [];
                viewModel.aantalList = [];
                $('input:checkbox:checked').each(function () {
                    var materiaalId = $(this).parent().find("input")[0].id;
                    //Indien het materiaal reeds in de lijst voorkomt.
                    if (viewModel.materiaalList.indexOf(parseInt(materiaalId)) < 0) {
                        viewModel.materiaalList.push(parseInt(materiaalId));
                        var aantal = $("#" + materiaalId).find($(".input-medium")).val();
                        if (parseInt(aantal) === 0) {
                            $(".foutmelding").text("Kies minstens 1 stuk van het geselecteerde materiaal!");
                            invalid = true;
                            return false;
                        }
                        viewModel.aantalList.push(parseInt(aantal));
                    }                                 
                });
                if (invalid) {
                    return false;
                }
                viewModel.session.setItem("materialen", JSON.stringify(viewModel.materiaalList));
                viewModel.session.setItem("aantal", JSON.stringify(viewModel.aantalList));
                viewModel.session.setItem("week", selectedWeek);
                viewModel.session.setItem("startDatum", viewModel.startDatum);
                viewModel.session.setItem("eindDatum", viewModel.eindDatum);
                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: "/Verlanglijst/Controle",
                    data: { materiaal: viewModel.materiaalList, aantal: viewModel.aantalList, startDatum:viewModel.startDatum, eindDatum: viewModel.eindDatum, knop : true},
                    success: function (data) {
                        $("#verlanglijst-pagina").html(data);
                        viewModel.init();
                    },
                });
            } else {
                $(".foutmelding").text("Selecteer een week!");
            }
        });
        $("#btn-reserveer").click(function () {

            $("#divLoading").addClass('toon');
            $("#divLoading").click(false);
            $(".navNotClick").click(false);
            var materialen = JSON.parse(viewModel.session.getItem("materialen"));
            var aantallen = JSON.parse(viewModel.session.getItem("aantal"));
            var selectedWeek = viewModel.session.getItem("week");
            var startDatum = viewModel.session.getItem("startDatum");
            var eindDatum = viewModel.session.getItem("eindDatum");
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Reservatie/MaakReservatie",
                data: { materiaal: materialen, aantal: aantallen, week: selectedWeek, startDatum: startDatum, eindDatum: eindDatum },
                success: function (data) {

                    $("#divLoading").hide();
                    window.location.href = '/catalogus/';
                }
            });
        });
        $("#btn-terug").click(function() {
            var materialen = JSON.parse(viewModel.session.getItem("materialen"));
            var aantallen = JSON.parse(viewModel.session.getItem("aantal"));
            var selectedWeek = viewModel.session.getItem("week");
            var startDatum = viewModel.session.getItem("startDatum");
            var eindDatum = viewModel.session.getItem("eindDatum");
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Verlanglijst/Controle",
                data: { materiaal: materialen, aantal: aantallen, week: selectedWeek, knop: false, startDatum: startDatum, eindDatum:eindDatum },
                success: function(data) {
                    $("#verlanglijst-pagina").html(data);
                    viewModel.init();
                }
            });
        });
    },
    getWeek: function (date) {
        var delen = date.toLocaleDateString().split("-");
        var newDate;
        if (delen[0] > 12) {
            newDate = new Date(delen[1] + "-" + delen[0] + "-" + delen[2]);
        } else {
            newDate = new Date(delen[0] + "-" + delen[1] + "-" + delen[2]);
        }
        return newDate.getWeek();
    },
    getDefaultDate : function() {
        return Date.parse('next monday');
    },
    getDaysOfNextWeek:function() {
        var dagen = [];
        dagen.push(Date.parse('next monday'));
        dagen.push(Date.parse('next tuesday'));
        dagen.push(Date.parse('next wednesday'));
        dagen.push(Date.parse('next thursday'));
        dagen.push(Date.parse('next friday'));
        return dagen;
    },
    getDaysOfWeek : function() {
        var dagen = [];
        dagen.push(Date.parse('monday'));
        dagen.push(Date.parse('tuesday'));
        dagen.push(Date.parse('wednesday'));
        dagen.push(Date.parse('thursday'));
        dagen.push(Date.parse('friday'));
        return dagen;
    }
}
function IsWeekend() {
    return Date.today().getDay() >= 6 || Date.today().getDay() === 0;
}
function VrijdagNaVijf() {
    return Date.today().getDay() === 5 && Date.today().getHours() >= 17;
}

function drawMaterial(dataMateriaal) {
    
    var data = new google.visualization.DataTable();
    var rows = new Array();
    data.addColumn('string', 'Startdatum');
    data.addColumn('string', 'Einddatum');
    data.addColumn('number', 'Aantal beschikbaar');
    $.each(data, function(i, item) {
        var startDatum = item.StartDatum;
        var eindDatum = item.EindDatum;
        var aantal = item.Aantal;
        rows.push([startDatum, eindDatum, aantal]);
    });
    //var obj = JSON.parse(dataMateriaal);
    //for (var key in obj) {
    //    if (obj.hasOwnProperty(key)) {
    //        var value = obj[key];
    //        rows.push([key, value]);
    //    }
    //}
    data.addRows(rows);
    var options = {
        chart: {
            title: 'Beschikbaarheid per week'
        },
        //hAxis: {
        //    title: 'Total Population',
        //    minValue: 0,
        //},
        //vAxis: {
        //    title: 'City'
        
        bars: 'horizontal'
    };
    var material = new google.charts.Bar(document.getElementById('chart_div'));
    material.draw(data, options);
}
$(document).ready(function() {
    viewModel.init();
})