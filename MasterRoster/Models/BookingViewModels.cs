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
            public int EmployeeId { get; set; }
            public string BookingType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Comment { get; set; }
        }

        public class BookingCellValidationViewModel
        {
            public bool IsValid { get; set; }
            public List<string> ValidationMessages { get; set; }
            public DateTime BookingDate { get; set; }
            public DayOfWeek BookingDay { get; set; }
            public string BookingCrew { get; set; }

            public Dictionary<DateTime,DayOfWeek> BookingDayDate {
                get{
                    var dayDate = new Dictionary<DateTime, DayOfWeek>();
                    dayDate.Add(BookingDate,BookingDay);
                    return dayDate;
                }
            }
        }


        public class BookingCellInfoViewModel
        {
            public List<Booking> Bookings { get; set; }
            public List<Employee> Employees { get; set; }
            public bool isShiftValid { get; set; }
            public List<string> ValidationMessages { get; set; }
            public string CrewCode { get; set; }
            public DateTime BookingDate { get; set; }
            public DayOfWeek BookingDay { get; set; }
            public int ManagersCount { set; get; }
            public int SupervisorsCount { set; get; }
            public int WorkersCounts { set; get; }
            public int GetEmployeeCountByRole(string role)
            {
                int count = 0;
                switch(role)
                {
                    case "Manager":
                        count = ManagersCount;
                        break;
                    case "Supervisor":
                        count = SupervisorsCount;
                        break;
                    case "Worker":
                        count = WorkersCounts;
                        break;
                }

                return count;
            }
        }
    }
}