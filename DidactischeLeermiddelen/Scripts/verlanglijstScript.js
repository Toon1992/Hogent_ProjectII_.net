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
    daysOfWeek : [],
    session : window.sessionStorage,
    selectedWeek : null,
    init: function () {
        //Nagaan of het op dit moment weekend is. Zoja, dan worden de dagen van de volgende week geblokkeerd.
        var weekend = viewModel.checkIsWeekend;
        var vrijdagNaVijf = viewModel.vrijdagNaVijf;
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
            var amount = 0;
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
            $('input:checkbox:checked').map(function () {
                amount++;
            });
        });
        $(".datecontrol").datepicker({
            changeMonth: true,
            changeYear: true,
            startDate: '+1d',
            daysOfWeekDisabled: [0,6],
            format: "dd-mm-yyyy",
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
                var aantal = $(this).parent().parent().parent().parent().find(".aantal").val();
                if (viewModel.materiaalList.indexOf(parseInt(materiaalId)) < 0) {
                    viewModel.materiaalList.push(parseInt(materiaalId));
                    viewModel.aantalList.push(parseInt(aantal));
                }
            });
            var selectedWeek = parseInt(new Date(viewModel.selectedWeek).getWeek());
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Verlanglijst/Controle",
                data: { materiaal: viewModel.materiaalList, aantal: viewModel.aantalList, week: selectedWeek, knop : false },
                success: function (data) {
                    $("#verlanglijst-pagina").html(data);
                    viewModel.init();
                }
            });
        });
        $("#btn-confirmeer").click(function () {
            if (viewModel.selectedWeek !== null) {
                viewModel.materiaalList = [];
                viewModel.aantalList = [];
                $('input:checkbox:checked').map(function () {
                    var materiaalId = $(this).parent().find("input")[0].id;
                    var aantal = $(this).parent().parent().parent().parent().find(".aantal").val();
                    if (viewModel.materiaalList.indexOf(parseInt(materiaalId)) < 0) {
                        viewModel.materiaalList.push(parseInt(materiaalId));
                        viewModel.aantalList.push(parseInt(aantal));
                    }                 
                });
                var selectedWeek = parseInt(new Date(viewModel.selectedWeek).getWeek());
                viewModel.session.setItem("materialen", JSON.stringify(viewModel.materiaalList));
                viewModel.session.setItem("aantal", JSON.stringify(viewModel.aantalList));
                viewModel.session.setItem("week", selectedWeek);
                $.ajax({
                    type: "POST",
                    traditional: true,
                    url: "/Verlanglijst/Controle",
                    data: { materiaal: viewModel.materiaalList, aantal: viewModel.aantalList, week: selectedWeek, knop : true},
                    success: function (data) {
                        $("#verlanglijst-pagina").html(data);
                        viewModel.init();
                    }
                });
            }
            
        });
        $("#btn-reserveer").click(function() {
            var materialen = JSON.parse(viewModel.session.getItem("materialen"));
            var aantallen = JSON.parse(viewModel.session.getItem("aantal"));
            var selectedWeek = viewModel.session.getItem("week");
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Verlanglijst/MaakReservatie",
                data: { materiaal: materialen, aantal: aantallen, week: selectedWeek },
                success: function(data) {

                }
            });
        });
        $("#btn-terug").click(function() {
            var materialen = JSON.parse(viewModel.session.getItem("materialen"));
            var aantallen = JSON.parse(viewModel.session.getItem("aantal"));
            var selectedWeek = viewModel.session.getItem("week");
            $.ajax({
                type: "POST",
                traditional: true,
                url: "/Verlanglijst/Controle",
                data: { materiaal: materialen, aantal: aantallen, week: selectedWeek, knop: false },
                success: function(data) {
                    $("#verlanglijst-pagina").html(data);
                    viewModel.init();
                }
            });
        });
    },
    checkIsWeekend : function() {
            //Als vandaag een weekdag is 
            if (today.getDay <= 5 && today.getDay !== 0) {
                return true;
            }
        return false;
    },
    vrijdagNaVijf : function() {
        if (today.getDay === 5 && today.getHours >= 17) {
            return true;
        }
        return false;
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
$(document).ready(function() {
    viewModel.init();
})