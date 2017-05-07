using MasterRoster.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterRoster.Models
{
    public class BookingForm_vm
    {
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string CrewCode { get; set; }
        public string bookingType { get; set; }
        public List<DayOfWeek> WeekDays { get; set; }
        public string RoleType { get; set; }

    }

}