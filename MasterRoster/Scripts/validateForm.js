function validateBookingForm()
{

    var errorMessages = "Please fix the following errors:<br /><br />";
    var result = true;

    if ($("#startDate").val() == "")
    {
        errorMessages += "* Start Date is empty<br />";
        result = false;
    }
    if ($("#endDate").val() == "") {

        errorMessages += "* End Date is empty<br />";
        result = false;
    }
    if($("#startDate").val() > $("#endDate").val())
    {
        errorMessages += "* Start Date is greater than End Date<br />";
        result = false;
    }
    if ($("#Comment").val().trim() == "")
    {
        errorMessages += "* Commnet can't be empty<br />";
        result = false;
    }

    $("#ErrorSummary").html(errorMessages);
    return result;
}