var viewModel = {
    materiaalList : [],
    init : function() {
        $("#verlanglijst-pagina .checkbox").change(function () {
            var amount = 0;
            //Selected row
            var materiaalId = $(this).find("input")[0].id;
            var materiaalRij = $("#" + materiaalId);
            if ($(this).children().is(":checked")) {
                //Het materiaalId
                viewModel.materiaalList.push(materiaalId);
                

                // highligt selected row    
                console.log(materiaalRij);
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
    }
}
$(document).ready(function() {
    viewModel.init();
})