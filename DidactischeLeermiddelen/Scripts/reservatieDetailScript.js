var viewModel = {
    init: function () {
        alert("its a go");
        $("detail-reservatie-header .datecontrol").datepicker({
            changeMonth: true,
            changeYear: true,
            startDate: '+1d',
            daysOfWeekDisabled: [0, 6],
            format: "dd-mm-yyyy",
            language: "nl",
            todayHighlight: true,
            calenderWeeks: true,
            beforeShowDay: function (date) {
                var dag = date.getDate();
                var maand = date.getMonth() + 1;
                var jaar = date.getFullYear();
                var datum = dag + '/' + maand + '/' + jaar;

                
            }
        }).on('changeDate', function (ev) {
            $(this).blur();
            $(this).datepicker('hide');
            var selectedWeek = viewModel.getWeek(Date.parse(ev.date));

        });
    }
}
$(document).ready(function() {
    viewModel.init();
});