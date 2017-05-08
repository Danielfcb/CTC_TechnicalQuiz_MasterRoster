function RedirectToWeeklyReport() {
    window.location.replace("/Bookings/WeeklyBookings?StartDate=" + $("#dateInWeek").val());
}

function RedirectToCreateBooking() {
    window.location.replace("/Bookings/Create");
}