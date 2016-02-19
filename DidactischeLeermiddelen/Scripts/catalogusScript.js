var ViewModel = {
    trefwoord: null,
    doelgroepen: [],
    leergebieden: [],
    filter : function() {
        ViewModel.doelgroepen = [];
        ViewModel.leergebieden = [];
        var doelgroepId = $("#doelgroepLijst").val();
        var leergebiedId = $("#leergebiedLijst").val();
        if (doelgroepId !== "") {
            ViewModel.doelgroepen.push(doelgroepId);
        }
        if (leergebiedId !== "") {
            ViewModel.leergebieden.push(leergebiedId);
        }
        
        $.ajax({
            type: "POST",
            traditional: true,
            url: "/Catalogus/Index",
            data: { doelgroepenLijst: ViewModel.doelgroepen, leergebiedenLijst: ViewModel.leergebieden },
            success: function (data) {
                $("#catalogus").html(data);
            }
        });
    },
    init: function () {
        $("#doelgroepLijst").change(function () {
            ViewModel.filter();
        });
        $("#leergebiedLijst").change(function () {
            ViewModel.filter();
        });
        $("#inhoud").keyup(function () {
            
            if ($("#inhoud").val() !== ViewModel.trefwoord) {
                ViewModel.trefwoord = $("#inhoud").val();
                $.get("/Catalogus/Index", { trefwoord: ViewModel.trefwoord }, function(data) {
                        $("#catalogus").html(data);
                });
            }
        });
        $("#zoekMobile").click(function () {
            
            $.get("/Catalogus/Index", { trefwoord: $("#inhoudMobile").val() }, function(data) {
                $("#catalogus").html(data);
            });
        });
        $("#zoek").click(function () {
            $.get("/Catalogus/Index", { trefwoord: $("#inhoud").val() }, function (data) {
                $("#catalogus").html(data);
            });
        });
        $("#catalogus-pagina .checkbox").change(function () {
            ViewModel.doelgroepen = [];
            ViewModel.leergebieden = [];
            $('input:checkbox:checked').map(function () {
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
                url: "/Catalogus/Index",
                data: { doelgroepenLijst: ViewModel.doelgroepen, leergebiedenLijst: ViewModel.leergebieden },
                success: function (data) {
                    $("#catalogus").html(data);
                }
            });
        });
        $(".manufacturer").click(function() {
            console.log($(this).text());
        });
        $(".materiaal-content").click(function () {
            if ($(window).width() < 768) {
                var materiaalId = $(this).find("img").attr("itemprop");
                $.get("/Catalogus/DetailView", { id: materiaalId }, function (data) {
                    $("#catalogus-pagina").html(data);
                });
            }

        });
    }
}
$(document).ready(function () {
    ViewModel.init();
})