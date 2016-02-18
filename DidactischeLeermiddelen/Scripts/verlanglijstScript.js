var viewModel = {
    materiaalList : [],
    init : function() {
        $("#verlanglijst-pagina .checkbox").change(function () {
            var amount = 0;
            //Selected row
            var x = $(this).parent().parent();
            if ($(this).children().is(":checked")) {
                // highligt selected row    
                var materiaalId = $(this).find("input")[0].id;
                viewModel.materiaalList.push(materiaalId);
                console.log($(this).find("input")[0].id);
                x.css("background-color", "#dff0d8");

                //Get id of selected material.
                var checkbox = $(this)[0];
                var id = parseInt(checkbox.children[0].id);
            } else {
                // dehighligt selected row
                x.css('background', 'transparent');
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