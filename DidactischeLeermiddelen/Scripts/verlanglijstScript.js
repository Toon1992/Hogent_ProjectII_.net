Date.prototype.getWeek = function() {
    var onejan = new Date(this.getFullYear(),0,1);
    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay()+1)/7);
}
var viewModel = {
    materiaalList: [],
    aantalList: [],
    selectedWeek : null,
    init : function() {
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
            if (amount === 0) {
                $("#reservatie-info").text("Selecteer materiaal om te reserveren");
            } else {
                $("#reservatie-info").text("Reserveer " + amount + " materialen");
            }
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
        });
        $("#btn-reserveer").click(function () {
            if (viewModel.selectedWeek !== null) {
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
                    url: "/Verlanglijst/Confirmatie",
                    data: { materiaal: viewModel.materiaalList, aantal: viewModel.aantalList, week: selectedWeek },
                    success: function (data) {

                    }
                });
            }
            
        });
    }
}
$(document).ready(function() {
    viewModel.init();
})