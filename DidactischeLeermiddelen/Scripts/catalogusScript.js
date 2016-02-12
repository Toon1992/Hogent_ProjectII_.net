var ViewModel = {
    init : function() {
        $("#doelgroepLijst").change(function() {
            $.get("Catalogus/Index", { doelgroepId: $("#doelgroepLijst").val(), leergebiedId: $("#leergebiedLijst").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $("#leergebiedLijst").change(function () {
            $.get("Catalogus/Index", { doelgroepId: $("#doelgroepLijst").val(), leergebiedId: $("#leergebiedLijst").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
    }
}
$(document).ready(function () {
    ViewModel.init();
})