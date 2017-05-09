function RedirectToWeeklyReport() {
    window.location.replace("/Bookings/BookingsTable?StartDate=" + $("#dateInWeek").val());
}

function RedirectToMonthlyReport()
{
    window.location.replace("/Bookings/BookingsTable?bookingPeriod=Monthly&StartDate=" + $("#dateInWeek").val());
}
function RedirectToCreateBooking() {
    window.location.replace("/Bookings/Create");
}