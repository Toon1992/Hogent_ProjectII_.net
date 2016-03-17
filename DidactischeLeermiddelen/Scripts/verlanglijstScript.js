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
    dagen: [],
    session: window.sessionStorage,
    dataGrafiek: null,
    init: function () {
        Cookies.init();
        $(document).keypress(function (event) {

            if (event.keyCode === 13) {
                event.preventDefault();
                $("#btn-confirmeer").click();
            }
        });
        var weekend = IsWeekend();
        var vrijdagNaVijf = VrijdagNaVijf();
        var dagen;
        if (weekend || vrijdagNaVijf) {
            dagen = viewModel.getDaysOfNextWeek();
            viewModel.daysOfWeek = $.map(dagen, function (date) {
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
        $(".input-medium").change(function () {
            var materiaalId = this.id;
            materiaalId = materiaalId.slice(-1);
            var materiaalRij = $("#" + materiaalId);
            var element = $.map($(".input-medium"), function (e) {
                var idd = e.id;
                if (idd.indexOf(materiaalId) > 0) {
                    return e;
                }
            });
            var value = parseInt(element[0].value);
            //Kijken of het item reeds geselecteerd was, zoniet selecteren en highligten
            var box = $("#verlanglijst-pagina .checkbox").find("." + materiaalId);
            if (!box.is(":checked") && value > 0) {
                box[0].checked = true;
                box[1].checked = true;
                materiaalRij.css("background-color", "#dff0d8");

            }
            if (box.is(":checked") && value === 0) {
                box[0].checked = false;
                box[1].checked = false;
                materiaalRij.css('background', 'transparent');
            }
            $("#btn-confirmeer").focus();
        });
        $("#reservatie-date").datepicker({
            changeMonth: true,
            changeYear: true,
            startDate: '+1d',
            daysOfWeekDisabled: [0, 6],
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
            viewModel.invoerControle(viewModel.materiaalList, viewModel.aantalList, startDatum, viewModel.dagen, false);
        });
        $("#reservatie-end-date").datepicker({
            changeMonth: true,
            changeYear: true,
            startDate: '+1d',
            daysOfWeekDisabled: [0, 6],
            format: "dd/mm/yyyy",
            language: "nl",
            todayHighlight: true,
            multidate: true,
            calenderWeeks: true,
        }).on('changeDate', function () {
            $(this).blur();
        }).on('hide', function () {
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
            var startDatum = $("input[name='multidate']")[0].value;
            if (startDatum !== "") {
                var dateStrings = startDatum.split(",");
                viewModel.dagen = dateStrings;
                viewModel.startDatum = dateStrings[0];
                viewModel.invoerControle(viewModel.materiaalList, viewModel.aantalList, viewModel.startDatum, viewModel.dagen, false);
            }


        });
        $(".detail-materiaal").click(function () {
            var materiaalId = $(this).parent().parent().find("input")[0].id;
            $.get("/Verlanglijst/ReservatieDetails", { id: materiaalId, week: -1 }, function (data) {
                $("#verlanglijst-pagina").html(data);
                $.getJSON("/Verlanglijst/ReservatieDetailsGrafiek", { id: materiaalId, week: -1 }, function (dataMateriaal) {
                    dataGrafiek = dataMateriaal;
                    google.charts.setOnLoadCallback(function () {
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
            if (typeof $("input[name='date']")[0] === "undefined" && viewModel.startDatum === null) {
                var data = $("input[name='multidate']")[0].value;
                if (data === "") {
                    $(".foutmelding").text("Selecteer een datum");
                    return false;
                } else {
                    viewModel.startDatum = data;
                }
                if (viewModel.dagen.length === 0) {
                    viewModel.dagen.push(viewModel.startDatum);
                }
            }
            if (typeof $("input[name='multidate']")[0] === "undefined") {
                viewModel.startDatum = $("input[name='date']")[0].value;
            }
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
            Cookies.create("dagen", JSON.stringify(viewModel.dagen), 1);
            if (viewModel.dagen.length === 0)
                viewModel.dagen = [];
            viewModel.invoerControle(viewModel.materiaalList, viewModel.aantalList, viewModel.startDatum, viewModel.dagen, true);
        });
        $("#btn-reserveer").click(function () {

            $("#divLoading").addClass('toon');
            $("#divLoading").click(false);
            $(".navNotClick").click(false);
            var materialen = JSON.parse(Cookies["materialen"]);
            var aantallen = JSON.parse(Cookies["aantal"]);
            var startDatum = Cookies["startDatum"];
            var dagen = JSON.parse(Cookies["dagen"]);
            if (materialen === "undefined" || aantallen === "undefined" || startDatum === "undefined" || dagen === "undefined") {
                //Someone fucked up the coockies, return to home
                window.location.href = '/verlanglijst/';
            }
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Reservatie/MaakReservatie",
                data: { materiaal: materialen, aantal: aantallen, startDatum: startDatum, dagen: dagen },
                success: function (data) {
                    $("#divLoading").hide();
                    window.location.href = '/catalogus/';
                }
            });
        });
        $("#btn-terug").click(function () {
            var materialen = JSON.parse(Cookies["materialen"]);
            var aantallen = JSON.parse(Cookies["aantal"]);
            var startDatum = Cookies["startDatum"];
            var dagen = JSON.parse(Cookies["dagen"]);
            if (dagen.length === 0) {
                dagen = [];
            }
            viewModel.invoerControle(materialen, aantallen, startDatum, dagen, false);

        });
    },
    invoerControle: function (materialen, aantallen, startDatum, dagen, knop) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: "/Verlanglijst/Controle",
            data: { materiaal: materialen, aantal: aantallen, naarReserveren: knop, startDatum: startDatum, dagen: dagen },
            success: function (data) {
                $("#verlanglijst-pagina").html(data);
                $.ajax({
                    type: "GET",
                    traditional: true,
                    url: "/Verlanglijst/ReservatieDetailsGrafiekPerDag",
                    data: { ids: materialen, dagen: dagen },
                    success: function (dataMateriaal) {
                        google.charts.setOnLoadCallback(function () {
                            drawMaterialPerDag(dataMateriaal);
                        });

                    }
                });
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
    getDefaultDate: function () {
        return Date.parse('next monday');
    },
    getDaysOfNextWeek: function () {
        var dagen = [];
        dagen.push(Date.parse('next monday'));
        dagen.push(Date.parse('next tuesday'));
        dagen.push(Date.parse('next wednesday'));
        dagen.push(Date.parse('next thursday'));
        dagen.push(Date.parse('next friday'));
        return dagen;
    },
    getDaysOfWeek: function () {
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
    return Date.today().is().saturday() || Date.today().is().sunday();
}
function VrijdagNaVijf() {
    return Date.today().is().friday() && new Date().getHours() >= 17;
}

function drawMaterial(dataMateriaal) {

    var data = new google.visualization.DataTable();
    var rows = new Array();
    data.addColumn('string', 'Startdatum');
    data.addColumn('number', 'Aantal beschikbaar');

    var obj = JSON.parse(dataMateriaal);
    $.each(obj, function (i, item) {
        var startDatum = item.StartDatum;

        var startDatumNaarDate = new Date(parseInt(startDatum.substr(6))).toLocaleDateString();

        var aantal = item.Aantal;
        rows.push([startDatumNaarDate, aantal]);
    });

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
function drawMaterialPerDag(dataMateriaal) {

    var obj = JSON.parse(dataMateriaal);
    var materiaalId;
    var options = {
        legend: { position: 'none' },
        chart: {
            title: 'Beschikbaarheid per dag',
        },
        backgroundColor: 'transparant'
    };
    //options.backgroundColor.fill = 'red';
    //Materialen overlopen
    var data = new google.visualization.DataTable();
    var rows = new Array();
    $.each(obj, function (i, item) {
        if (item.length !== 0) {
            rows = new Array();
            data.addColumn('string', 'Dagen');
            data.addColumn('number', 'Aantal beschikbaar');
            //De verschillende dagen voor het materiaal
            $.each(item, function (j, grafiek) {
                var startDatum = grafiek.StartDatum;
                var startDatumNaarDate = new Date(parseInt(startDatum.substr(6))).toLocaleDateString();
                var aantal = grafiek.Aantal;
                materiaalId = grafiek.MateriaalId;
                rows.push([startDatumNaarDate, aantal]);
            });
            data.addRows(rows);
            var material = new google.charts.Bar(document.getElementById('Grafiek_dag_' + materiaalId));
            material.draw(data, google.charts.Bar.convertOptions(options));
        }
    });

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

//$(window).resize(function () {
//    drawMaterial(dataGrafiek);
//});

$(document).ready(function () {
    google.charts.load('current', { packages: ['corechart', 'bar'], 'language': 'nl' });
    viewModel.init();
})

