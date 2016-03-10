//Date.prototype.getWeek = function () {
//    var onejan = new Date(this.getFullYear(),0,1);
//    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay()+1)/7);
//}
//Date.prototype.addDays = function (days) {
//    var dat = new Date(this.valueOf());
//    dat.setDate(dat.getDate() + days);
//    return dat;
//}
var Cookies = {
    init: function () {
        var allCookies = document.cookie.split('; ');
        for (var i = 0; i < allCookies.length; i++) {
            var cookiePair = allCookies[i].split('=');
            this[cookiePair[0]] = cookiePair[1];
        }
    },
    create: function (name, value, days) {
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            var expires = "; expires=" + date.toGMTString();
        }
        else var expires = "";
        document.cookie = name + "=" + value + expires + "; path=/";
        this[name] = value;
    },
    erase: function (name) {
        this.create(name, '', -1);
        this[name] = undefined;
    }
};
var viewModel = {
    materiaalList: [],
    aantalList: [],
    daysOfWeek: [],
    startDatum: null,
    eindDatum: null,
    session: window.sessionStorage,
    dataGrafiek : null,
    init: function () {
        Cookies.init();
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
            $("#btn-confirmeer").focus();
            //Wanneer een van de twee checkboxen aangeklikt wordt, krijgt de tweede dezelfde waarde als de eerste
            var box = $("#verlanglijst-pagina .checkbox").find("." + materiaalId);
            var selectedBox = $(this).find("input")[0].checked;
            box[0].checked = selectedBox;
            box[1].checked = selectedBox;
        });
        $(".input-medium").change(function() {
            var materiaalId = this.id;
            materiaalId = materiaalId.slice(-1);
            var materiaalRij = $("#" + materiaalId);
            //Kijken of het item reeds geselecteerd was, zoniet selecteren en highligten
            var box = $("#verlanglijst-pagina .checkbox").find("." + materiaalId);
            if (!box.is(":checked")) {
                box[0].checked = true;
                box[1].checked = true;
                if ($("#verlanglijst-pagina .checkbox").find("." + materiaalId).is(":checked")) { 
                    materiaalRij.css("background-color", "#dff0d8");
                } else {
                    materiaalRij.css('background', 'transparent');
                }
            }
            $("#btn-confirmeer").focus();
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
            var startDatum = $("input[name='date']")[0].value;
            viewModel.invoerControle(viewModel.materiaalList, viewModel.aantalList, startDatum, viewModel.eindDatum, false);
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
            "minDate": Date.parse("today").toLocaleDateString()
        }).on('apply.daterangepicker', function (ev, picker) {
            viewModel.startDatum = picker.startDate.format("DD/MM/YYYY");
            viewModel.eindDatum = picker.endDate.format("DD/MM/YYYY");
            $('input:checkbox:checked').map(function () {
                var materiaalId = $(this).parent().find("input")[0].id;
                var aantal = $("#" + materiaalId).find($(".input-medium")).val();
                if (viewModel.materiaalList.indexOf(parseInt(materiaalId)) < 0) {
                    viewModel.materiaalList.push(parseInt(materiaalId));
                    viewModel.aantalList.push(parseInt(aantal));
                }
            });
            viewModel.invoerControle(viewModel.materiaalList, viewModel.aantalList, viewModel.startDatum, viewModel.eindDatum, false);
        });
        $(".detail-materiaal").click(function () {
            var materiaalId = $(this).parent().parent().find("input")[0].id;
            $.get("/Verlanglijst/ReservatieDetails", { id: materiaalId, week: -1 }, function (data) {
                $("#verlanglijst-pagina").html(data);
                $.getJSON("/Verlanglijst/ReservatieDetailsGrafiek", { id: materiaalId, week: -1 }, function (dataMateriaal) {
                    dataGrafiek = dataMateriaal;
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
        }).on('changeDate', function () {
            $(this).blur();
            $(this).datepicker('hide');
            var date = Date.parse($("input[name='date']")[0].value);
            var selectedWeek = viewModel.getWeek(date);
            var materiaalId = parseInt($(".materiaal-naam")[0].id);
            $.get("/Verlanglijst/ReservatieDetails", { id: materiaalId, week: selectedWeek }, function (data) {
                $("#verlanglijst-pagina").html(data);
                $.getJSON("/Verlanglijst/ReservatieDetailsGrafiek", { id: materiaalId, week: selectedWeek }, function (dataMateriaal) {
                    google.charts.setOnLoadCallback(function () {
                        drawMaterial(dataMateriaal);
                    });
                });
                viewModel.init();
            });
        });
        $("#btn-confirmeer").click(function () {
            var invalid;
            var selectedWeek;
            if (typeof  $("input[name='date']")[0] !== "undefined") {
                viewModel.startDatum = $("input[name='date']")[0].value;
            } else {
                var datums = $("input[name='daterange']")[0].value;
                var delen = datums.split("-");
                viewModel.startDatum = delen[0];
                viewModel.eindDatum = delen[1];
                viewModel.selectedWeek = viewModel.startDatum + "-" + viewModel.eindDatum;
            }
            
           
            if (viewModel.selectedWeek !== "-undefined") {
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
                Cookies.create("materialen", JSON.stringify(viewModel.materiaalList), 1);
                Cookies.create("aantal", JSON.stringify(viewModel.aantalList), 1);
                Cookies.create("startDatum", viewModel.startDatum, 1);
                Cookies.create("eindDatum", viewModel.eindDatum, 1);
                viewModel.invoerControle(viewModel.materiaalList, viewModel.aantalList, viewModel.startDatum, viewModel.eindDatum, true);
            } else {
                $(".foutmelding").text("Selecteer een week!");
            }
        });
        $("#btn-reserveer").click(function () {

            $("#divLoading").addClass('toon');
            $("#divLoading").click(false);
            $(".navNotClick").click(false);
            var materialen = JSON.parse(Cookies["materialen"]);
            var aantallen = JSON.parse(Cookies["aantal"]);
            var startDatum = Cookies["startDatum"];
            var eindDatum = Cookies["eindDatum"];
            if (materialen === "undefined" || aantallen === "undefined" || startDatum === "undefined" || eindDatum === "undefined") {
                //Someone fucked up the coockies, return to home and give a message
                window.location.href = '/verlanglijst/';
            }
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Reservatie/MaakReservatie",
                data: { materiaal: materialen, aantal: aantallen, startDatum: startDatum, eindDatum: eindDatum },
                success: function (data) {
                    $("#divLoading").hide();
                    window.location.href = '/catalogus/';
                }
            });
        });
        $("#btn-terug").click(function() {
            var materialen = JSON.parse(Cookies["materialen"]);
            var aantallen = JSON.parse(Cookies["aantal"]);
            var startDatum = Cookies["startDatum"];
            var eindDatum = Cookies["eindDatum"];
            viewModel.invoerControle(materialen, aantallen, startDatum, eindDatum, false);
        });
    },
    invoerControle : function(materialen,aantallen, startDatum, eindDatum, knop) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: "/Verlanglijst/Controle",
            data: { materiaal: materialen, aantal: aantallen, knop: knop, startDatum: startDatum, eindDatum: eindDatum },
            success: function (data) {
                $("#verlanglijst-pagina").html(data);
                viewModel.init();
            }
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
    data.addColumn('date', 'Startdatum');
    data.addColumn('number', 'Aantal beschikbaar');
    
    var obj = JSON.parse(dataMateriaal);
    $.each(obj, function(i, item) {
        var startDatum = item.StartDatum;

        var startDatumNaarDate = new Date(parseInt(startDatum.substr(6)));
        console.log(typeof startDatumNaarDate);
            var aantal = item.Aantal;
            rows.push([startDatumNaarDate, aantal]);
        });
    //for (var dataTest in obj) {
    //    var aantal = dataTest.aantal;
    //    var startDatum = dataTest.startDatum;
        
    //    rows.push([aantal, startDatum]);
    //}
    //$.each(dataMateriaal, function(i, item) {
    //    var startDatum = item.StartDatum;
    //    console.log(typeof startDatum);
    //    var aantal = item.Aantal;
    //    rows.push([aantal, startDatum]);
    //});
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
        hAxis: {
            title: 'Aantal beschikbaar'
        //    minValue: 0,
        },
        vAxis: {
            title: 'Startdatum',
            bars: 'horizontal'
        }
    };
    var material = new google.charts.Bar(document.getElementById('chart_div'));
    material.draw(data, options);
}
dateTimeReviver = function (key, value) {
    var a;
    if (typeof value === 'string') {
        a = /\/Date\((\d*)\)\//.exec(value);
        if (a) {
            return new Date(+a[1]);
        }
    }
    return value;
}

$(window).resize(function () {
    drawMaterial(dataGrafiek);
});

$(document).ready(function () {
    google.charts.load('current', { packages: ['corechart', 'bar'], 'language' : 'nl'});
    viewModel.init();
})

