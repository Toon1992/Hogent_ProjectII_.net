var viewModel = {
    trefwoord: null,
    doelgroepen: [],
    leergebieden: [],
    filter : function() {
        viewModel.doelgroepen = [];
        viewModel.leergebieden = [];
        var doelgroepId = $("#doelgroepLijst").val();
        var leergebiedId = $("#leergebiedLijst").val();
        if (doelgroepId !== "") {
            viewModel.doelgroepen.push(doelgroepId);
        }
        if (leergebiedId !== "") {
            viewModel.leergebieden.push(leergebiedId);
        }
        
        $.ajax({
            type: "POST",
            traditional: true,
            url: "/Catalogus/Index",
            data: { doelgroepenLijst: viewModel.doelgroepen, leergebiedenLijst: viewModel.leergebieden },
            success: function (data) {
                $("#catalogus").html(data);
            }
        });
    },
    init: function () {
        $("#doelgroepLijst").change(function () {
            viewModel.filter();
        });
        $("#leergebiedLijst").change(function () {
            viewModel.filter();
        });
        $("#inhoud").keyup(function () {
            
            if ($("#inhoud").val() !== viewModel.trefwoord) {
                viewModel.trefwoord = $("#inhoud").val();
                $.get("/Catalogus/Index", { trefwoord: viewModel.trefwoord }, function(data) {
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
            viewModel.doelgroepen = [];
            viewModel.leergebieden = [];
            $('input:checkbox:checked').map(function () {
                var filter = $(this).parent().parent()[0].className;
                var id = $(this)[0].id;//parseInt(filter.substr(filter.length - 1));
                if (filter.indexOf("doelgroep") > -1) {
                        viewModel.doelgroepen.push(id);
                }
                if (filter.indexOf("leergebied") > -1) {
                        viewModel.leergebieden.push(id);
                }
            });
            $.ajax({
                type: "POST",
                traditional: true, 
                url: "/Catalogus/Index",
                data: { doelgroepenLijst: viewModel.doelgroepen, leergebiedenLijst: viewModel.leergebieden },
                success: function (data) {
                    $("#catalogus").html(data);
                }
            });
        });
        $(".manufacturer").click(function () {
            var id = $(this).parent().parent().parent().parent().find("img").attr("itemprop");
            //$(".tempdatas").html("");
            $.get("/Catalogus/DetailViewFirma", { id: id }, function (data) {
                $("#catalogus-pagina").html(data);
            });
        });
        $(".materiaal-content").click(function (e) {
            if ($(window).width() < 768) {
                var manufacturer = $(e.target).attr('class').indexOf("manufacturer");
                if (manufacturer < 0) {
                    var materiaalId = $(this).find("img").attr("itemprop");
                    $.get("/Catalogus/DetailView", { id: materiaalId }, function (data) {
                        $("#catalogus-pagina").html(data);
                    });
                }
                
            }

        });
    }
}
$(document).ready(function () {
    viewModel.init();
})