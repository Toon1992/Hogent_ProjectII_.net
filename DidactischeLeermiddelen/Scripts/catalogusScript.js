var ViewModel = {
    trefwoord: null,
    init : function() {
        $("#doelgroepLijst").change(function () {
            $.get("Catalogus/Index", { doelgroepId: $("#doelgroepLijst").val(), leergebiedId: $("#leergebiedLijst").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $("#leergebiedLijst").change(function () {
            $.get("Catalogus/Index", { doelgroepId: $("#doelgroepLijst").val(), leergebiedId: $("#leergebiedLijst").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $("#inhoud").keyup(function () {
            
            if ($("#inhoud").val() !== ViewModel.trefwoord) {
                ViewModel.trefwoord = $("#inhoud").val();
                $.get("Catalogus/Zoek", { trefwoord: ViewModel.trefwoord }, function(data) {
                        $("#catalogus").html(data);
                });
            }

        });
        $(".checkbox").change(function() {

            var input = $(this).children()[0];
            var filter = input.id;
                if (filter.indexOf("doelgroep") > -1) {
                    filter = parseInt(filter.substr(filter.length - 1));
                    $.get("Catalogus/Index", { doelgroepId: filter, leergebiedId: 0 }, function (data) {
                        $("#catalogus").html(data);
                    });
                } else {
                    filter = parseInt(filter.substr(filter.length - 1));
                    $.get("Catalogus/Index", { doelgroepId: 0, leergebiedId: filter }, function (data) {
                        $("#catalogus").html(data);
                    });
                }
        });
    }
}
$(document).ready(function () {
    ViewModel.init();
})