Date.prototype.getWeek = function() {
    var onejan = new Date(this.getFullYear(),0,1);
    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay()+1)/7);
}
var viewModel = {
    materiaalList: [],
    aantalList: [],
    session : window.sessionStorage,
    selectedWeek : null,
    init: function () {
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
            format: "dd-mm-yyyy",
            language: "tr"
        }).on('changeDate', function (ev) {
            $(this).blur();
            $(this).datepicker('hide');
            viewModel.selectedWeek = ev.date;
            viewModel.materiaalList = [];
            viewModel.aantalList = [];
            $('input:checkbox:checked').map(function () {
                var materiaalId = $(this).parent().find("input")[0].id;
                var aantal = $(this).parent().parent().parent().parent().find(".aantal").val();
                viewModel.materiaalList.push(parseInt(materiaalId));
                viewModel.aantalList.push(parseInt(aantal));
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
                        viewModel.materiaalList.push(parseInt(materiaalId));
                        viewModel.aantalList.push(parseInt(aantal));                  
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
    }
}
$(document).ready(function() {
    viewModel.init();
})