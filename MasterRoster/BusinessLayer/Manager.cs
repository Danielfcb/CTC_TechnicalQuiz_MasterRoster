using MasterRoster.Common;
using MasterRoster.DataAccessLayer;
using MasterRoster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using static MasterRoster.Models.BookingViewModels;

namespace MasterRoster.BusinessLayer
{
    public class Manager
    {
        private MasterRosterEntities _db;

        public Manager()
        {
            _db = new MasterRosterEntities();
        }

        public BookingCellInfoViewModel GetBookingCellInfo(string crew, DateTime date)
        {

            BookingCellInfoViewModel bookingCellInfo = new BookingCellInfoViewModel();
            bookingCellInfo.CrewCode = crew;
            bookingCellInfo.BookingDate = date;
            bookingCellInfo.BookingDay = date.DayOfWeek;

            bookingCellInfo.Bookings = _db.Bookings.Include("Employee").Where(b => b.start_date <= date && b.end_date >= date && b.Employee.Crew.crew_code == crew).ToList();
            bookingCellInfo.Employees = bookingCellInfo.Bookings.Select(b => b.Employee).ToList();
            List<string> validationMessages;
            bookingCellInfo.isShiftValid = ValidateBookingCell(date, crew, out validationMessages);
            bookingCellInfo.ValidationMessages = validationMessages;
            bookingCellInfo.ManagersCount = _db.Employees.Where(e => e.RoleType.roletype_name == "Manager").Count();
            bookingCellInfo.SupervisorsCount = _db.Employees.Where(e => e.RoleType.roletype_name == "Supervisor").Count();
            bookingCellInfo.WorkersCounts = _db.Employees.Where(e => e.RoleType.roletype_name == "Worker").Count();


            return bookingCellInfo;
        }
         
        public List<BookingCellValidationViewModel> GetWeeklyBookingCellResults(DateTime dayOfWeek)
        {
            List<BookingCellValidationViewModel> viewModel = new List<BookingCellValidationViewModel>();
            DateTime startingMonday;
            DateTime endingMonday;
            Utilities.GetCurrentWeek(dayOfWeek, out startingMonday, out endingMonday);
            Dictionary<DayOfWeek, DateTime> weekDates = new Dictionary<DayOfWeek, DateTime>();
            for (DateTime day = startingMonday;day < endingMonday; day = day.AddDays(1) )
            {
                weekDates.Add(day.DayOfWeek, day);
            }
            foreach(var day in weekDates)
            {
                
                foreach( var crew in GetAllDistinctCrews())
                {
                    BookingCellValidationViewModel model = new BookingCellValidationViewModel();
                    model.BookingDay = day.Key;
                    model.BookingDate = day.Value;
                    model.BookingCrew = crew;
                    List<string> validationMessages;
                    model.IsValid = ValidateBookingCell(day.Value, crew, out validationMessages);
                    model.ValidationMessages = validationMessages;
                    viewModel.Add(model);
                }               
            }

            return viewModel;
        }

        internal void InsertBookingToDatabase(BookingViewModels.BookingAdd form)
        {
            Booking booking = new Booking()
            {
                comment = form.Comment,
                employee_id = form.Employee_Id,
                start_date = Convert.ToDateTime(form.StartDate),
                end_date = Convert.ToDateTime(form.EndDate),
                deleted_flag = false,
                booking_type_code = form.BookingType,
                
            };
            _db.Bookings.Add(booking);
            _db.SaveChanges();
        }

        public BookingType GetBookingTypeByName(string bookingTypeName)
        {
            return _db.BookingTypes.Where(bt => bt.booking_type_name == bookingTypeName).SingleOrDefault();
        }

        public List<string> GetAllDistinctCrews()
        {
            return _db.Crews.Distinct().Select(c => c.crew_code).ToList();
        }
        public  List<BookingType> GetAllBookingTypes()
        {
            return _db.BookingTypes.ToList();
        }

        public Employee GetEmployeeById(int? employeeId)
        {
            Employee employee = null;
            if(employeeId != null && employeeId != 0)
            {
                employee = _db.Employees.Where(e => e.employee_id == employeeId).SingleOrDefault();
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

        public bool ValidateBookingCell(DateTime day, string crew, out List<string> ValidationMessages)
        {
            bool result = true;
            ValidationMessages = new List<string>();
            var allBookingsForTheCell = _db.Bookings.Where(b => b.start_date <= day && day <= b.end_date & b.Employee.Crew.crew_code == crew).ToList();
            if(allBookingsForTheCell.Where(b => b.Employee.RoleType.roletype_name == "Manager").Count() < 0.5 * CountAllManagers())
            {
                ValidationMessages.Add("Not more than %50 of Managers can be scheduled off for a shift");
                result = false;
            }
            if (allBookingsForTheCell.Where(b => b.Employee.RoleType.roletype_name == "Manager").Count() < 0.5 * CountAllManagers())
            {
                ValidationMessages.Add("Not more than %50 of Supervisors can be scheduled off for a shift");
                result = false;
            }
            if (allBookingsForTheCell.Where(b => b.Employee.RoleType.roletype_name == "Worker").Count() < 0.75 * CountAllWorkers())
            {
                ValidationMessages.Add("Not more than %25 of Workers can be scheduled off for a shift");

                result = false;
            }
            return result;
        }

        public int CountAllManagers()
        {
            return _db.Employees.Where(e => e.RoleType.roletype_name == "Manager").Count();
        }


        public int CountAllWorkers()
        {
            return _db.Employees.Where(e => e.RoleType.roletype_name == "Worker").Count();
        }
    }
}