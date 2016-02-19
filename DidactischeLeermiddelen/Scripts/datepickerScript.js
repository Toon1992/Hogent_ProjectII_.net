$(document).ready(function () {
    $(".datecontrol").datepicker({
        changeMonth: true,
        changeYear: true,
        format: "dd-mm-yyyy",
        language: "tr"
    }).on('changeDate', function (ev) {
        $(this).blur();
        $(this).datepicker('hide');
        alert(ev.date);
    });
    $("#dp3").datepicker({
        changeMonth: true,
        changeYear: true,
        format: "dd-mm-yyyy",
        language: "tr"
    }).on('changeDate', function (ev) {
        $(this).blur();
        $(this).datepicker('hide');
        alert(ev.date);
    });
})