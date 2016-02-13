var ViewModel = {
    trefwoord: null,
    doelgroepen: [],
    leergebieden: [],
    init : function() {
        $("#doelgroepLijst").change(function () {
            $.get("/Catalogus/FilterMobile", { doelgroepId: $("#doelgroepLijst").val(), leergebiedId: $("#leergebiedLijst").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $("#leergebiedLijst").change(function () {
            $.get("/Catalogus/FilterMobile", { doelgroepId: $("#doelgroepLijst").val(), leergebiedId: $("#leergebiedLijst").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $("#inhoud").keyup(function () {
            
            if ($("#inhoud").val() !== ViewModel.trefwoord) {
                ViewModel.trefwoord = $("#inhoud").val();
                $.get("/Catalogus/Zoek", { trefwoord: ViewModel.trefwoord }, function(data) {
                        $("#catalogus").html(data);
                });
            }
        });
        $("#zoekMobile").click(function () {
            
            $.get("/Catalogus/Zoek", { trefwoord: $("#inhoudMobile").val() }, function(data) {
                $("#catalogus").html(data);
            });
        });
        $("#zoek").click(function () {
            $.get("/Catalogus/Zoek", { trefwoord: $("#inhoud").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $(".checkbox").change(function () {
            ViewModel.doelgroepen = [];
            ViewModel.leergebieden = [];
            $('input:checkbox:checked').map(function() {
                var filter = $(this)[0].id;
                var id = parseInt(filter.substr(filter.length - 1));
                if (filter.indexOf("doelgroep") > -1) {
                        ViewModel.doelgroepen.push(id);
                }
                if (filter.indexOf("leergebied") > -1) {
                        ViewModel.leergebieden.push(id);
                }
            });
            $.ajax({
                type: "POST",
                traditional: true, 
                url: "/Catalogus/Filter",
                data: { doelgroepenLijst: ViewModel.doelgroepen, leergebiedenLijst: ViewModel.leergebieden },
                success: function (data) {
                    $("#catalogus").html(data);
                }
            });
            //var input = $(this).children()[0];
            //var filter = input.id;
            //    if (filter.indexOf("doelgroep") > -1) {
            //        filter = parseInt(filter.substr(filter.length - 1));
            //        $.get("Catalogus/Index", { doelgroepId: filter, leergebiedId: 0 }, function (data) {
            //            $("#catalogus").html(data);
            //        });
            //    } else {
            //        filter = parseInt(filter.substr(filter.length - 1));
            //        $.get("Catalogus/Index", { doelgroepId: 0, leergebiedId: filter }, function (data) {
            //            $("#catalogus").html(data);
            //        });
            //    }
        });
    }
}
$(document).ready(function () {
    ViewModel.init();
})