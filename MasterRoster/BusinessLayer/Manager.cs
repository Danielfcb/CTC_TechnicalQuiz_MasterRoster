using MasterRoster.Common;
using MasterRoster.DataAccessLayer;
using MasterRoster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace MasterRoster.BusinessLayer
{
    public class Manager
    {
        private MasterRosterEntities _db;

        public Manager()
        {
            _db = new MasterRosterEntities();
        }

        public List<BookingForm_vm> GetCurrentWeeksBooking()
        {
            List<BookingForm_vm> bookingList = new List<BookingForm_vm>();
            DateTime startingMonday;
            DateTime endingMonday;
            Utilities.GetCurrentWeek(DateTime.Today, out startingMonday, out endingMonday);

            var bookings = _db.Bookings.Where(b => b.start_date > startingMonday && b.end_date < endingMonday).ToList();

            foreach (var booking in bookings)
            {
                BookingForm_vm bookingFormModel = new BookingForm_vm()
                {
                    bookingType = booking.BookingType.booking_type_name,
                    CrewCode = booking.Employee.Crew.crew_code,
                    EmployeeName = booking.Employee.name,
                    EmployeeNumber = booking.Employee.employee_num,
                    RoleType = booking.Employee.RoleType.roletype_name,
                    WeekDays = GetDaysForDateRange(booking.start_date, booking.end_date)
                };

                bookingList.Add(bookingFormModel);
            }
            

            return bookingList;
        }

        internal void InsertBookingToDatabase(BookingViewModels.BookingAdd form)
        {
            throw new NotImplementedException();
        }

        public  List<BookingType> GetAllBookingTypes()
        {
            return _db.BookingTypes.ToList();
        }

        public Employee GetEmployeeByNumber(string employeeNum)
        {
            Employee employee = null;
            if(!string.IsNullOrEmpty(employeeNum))
            {
                employee = _db.Employees.Where(e => e.employee_num == employeeNum).SingleOrDefault();
            }
            return employee;
        }

        public List<DayOfWeek> GetDaysForDateRange(DateTime startDate, DateTime endDate)
        {
            DateTime day = startDate;
            List<DayOfWeek> weekDaysList = new List<DayOfWeek>();
            while(day < endDate)
            {
                weekDaysList.Add(day.DayOfWeek);
                day = day.AddDays(1);
            }

            return weekDaysList;
        }

        public List<Employee> GetAllEmployees()
        {
            return _db.Employees.ToList();
        }
        public List<Employee> GetTop100Employees()
        {
            return _db.Employees.Take(10).ToList();
        }
    }
}