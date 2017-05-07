using MasterRoster.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterRoster.Models
{
    public class BookingViewModels
    {
        public class BookingAddForm
        {
            public SelectList Employees { get; set; }

            public SelectList BookingTypes { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class BookingAdd
        {
            public string EmployeeNum { get; set; }
            public string BookingType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Comment { get; set; }
        }
    }
}