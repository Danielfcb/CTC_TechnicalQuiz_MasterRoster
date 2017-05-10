function RedirectToWeeklyReport() {
    window.location.replace("/Bookings/BookingsTable?StartDate=" + $("#date").val());
}

function RedirectToMonthlyReport()
{
    window.location.replace("/Bookings/BookingsTable?bookingPeriod=Monthly&StartDate=" + $("#date").val());
}
function RedirectToCreateBooking() {
    window.location.replace("/Bookings/Create");
}

function SearchEmployees()
{
    window.location.replace("/Employees/Index?crew=" + $("#Crew option:selected").text() + "&role=" + $("#Role option:selected").text());
}