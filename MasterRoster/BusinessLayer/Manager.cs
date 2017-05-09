using MasterRoster.Common;
using MasterRoster.DataAccessLayer;
using MasterRoster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using static MasterRoster.Models.BookingViewModels;
using System.Web.Mvc;

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
         
        public List<BookingCellValidationViewModel> GetBookingCellResults(DateTime date, string bookingPeriod)
        {
            List<BookingCellValidationViewModel> viewModel = new List<BookingCellValidationViewModel>();
            DateTime startingDate;
            DateTime endingDate;
            if (bookingPeriod == "Weekly")
                Utilities.GetCurrentWeek(date, out startingDate, out endingDate);
            else
                Utilities.GetCurrentMonth(date, out startingDate, out endingDate);

            Dictionary<DayOfWeek, DateTime> weekDates = new Dictionary<DayOfWeek, DateTime>();

            for(var day = startingDate; day < endingDate; day = day.AddDays(1))
            {
                
                foreach( var crew in GetAllDistinctCrews())
                {
                    BookingCellValidationViewModel model = new BookingCellValidationViewModel();
                    model.BookingDay = day.DayOfWeek;
                    model.BookingDate = day;
                    model.BookingCrew = crew;
                    List<string> validationMessages;
                    model.IsValid = ValidateBookingCell(day, crew, out validationMessages);
                    model.ValidationMessages = validationMessages;
                    viewModel.Add(model);
                }               
            }

            return viewModel;
        }

        internal BookingAddForm CreateBookingAddForm()
        {
            var allEmployees = GetAllEmployees();
            var allBookingTypes = GetAllBookingTypes();

            var bookingAddForm = new BookingAddForm()
            {
                BookingTypes = new SelectList(allBookingTypes, "booking_type_code", "booking_type_name"),
                Employees = new SelectList(allEmployees, "employee_id", "name")
            };
            return bookingAddForm;
        }

        public bool InsertBookingToDatabase(BookingAdd form, out List<string>validationMessages)
        {
            bool result = false;
            if (IsBookingValid(form, out validationMessages))
            {
                try
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
                    result = true;
                }
                catch(Exception ex)
                {
                    result = false;
                    LogWriter.WriteLog(ex);
                }
            }

            return result;
        }

        public bool UpdateBooking(BookingEdit updatedBooking, out List<string> validationMessages)
        {
            bool result = false;
            validationMessages = new List<string>();
            if(updatedBooking != null)
            {
                if(IsUpdatedBookingValid(updatedBooking, out validationMessages))
                {
                    try
                    {


                        Booking bookingToReplace = _db.Bookings.Where(b => b.booking_id == updatedBooking.BookingId).SingleOrDefault();
                        bookingToReplace.booking_type_code = updatedBooking.BookingType;
                        bookingToReplace.comment = updatedBooking.Comment;
                        bookingToReplace.start_date = updatedBooking.StartDate;
                        bookingToReplace.end_date = updatedBooking.EndDate;

                        _db.SaveChanges();

                        result = true;
                    }
                    catch(Exception ex)
                    {
                        result = false;
                        LogWriter.WriteLog(ex);
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public bool IsUpdatedBookingValid(BookingEdit updatedBooking, out List<string> validationMessages)
        {
            bool result = true;
            validationMessages = new List<string>();
            if (GetEmployeeById(updatedBooking.EmployeeId) == null)
            {
                result = false;
                validationMessages.Add("Employee ID " + updatedBooking.EmployeeId + "Doesn't exist!");
            }
            if(result)
            {
                List<Booking> allBookings = GetAllBookingsForEmployeeId(updatedBooking.EmployeeId);
                List<Booking> Conflicts = allBookings.Where(b => b.booking_id != updatedBooking.BookingId && ((updatedBooking.StartDate > b.start_date && updatedBooking.StartDate < b.end_date) || (updatedBooking.StartDate < b.start_date && updatedBooking.EndDate > b.start_date))).ToList();
                result = Conflicts.Count() == 0;
                if(!result)
                {
                    validationMessages.Add("Booking requested starting on " + updatedBooking.StartDate.ToString("MM/dd/yyyy") +
                                           " and ending on " + updatedBooking.EndDate.ToString("MM/dd/yyyy") + " has conflict with the following booking(s)!");
                    foreach (var conflict in Conflicts)
                    {
                        validationMessages.Add("Booking ID: " + conflict.booking_id + " starting on " + conflict.start_date.ToString("MM/dd/yyyy") +
                                               " ending on " + conflict.end_date.ToString("MM/dd/yyyy"));
                    }
                }
            }
            

            return result;
        }
        public BookingEditForm CreateBookingEditForm(int bookingId)
        {
            BookingEditForm form = new BookingEditForm();
            Booking selectedBooking = _db.Bookings.Where(b => b.booking_id == bookingId).SingleOrDefault();
            if(selectedBooking!=null)
            {
                form.BookingId = selectedBooking.booking_id;
                form.EmployeeName = selectedBooking.Employee.name;
                form.EmployeeNumber = selectedBooking.Employee.employee_num;
                form.EmployeeId = selectedBooking.Employee.employee_id;
                form.StartDate = selectedBooking.start_date;
                form.EndDate = selectedBooking.end_date;
                form.BookingType = new SelectList(GetAllBookingTypes(), "booking_type_code", "booking_type_name", selectedBooking.BookingType.booking_type_code);
                form.Comment = selectedBooking.comment;
            }
            
            return form;
        }

        public bool IsBookingValid(BookingAdd newBooking, out List<string> validationMessages)
        {
            bool result = true;
            validationMessages = new List<string>();
            // Check if the employee exists
            if(GetEmployeeById(newBooking.Employee_Id) == null)
            {
                result = false;
                validationMessages.Add("Employee ID " + newBooking.Employee_Id + "Doesn't exist!");
            }
            //Check if there is any conflicts with existing bookings for the employee
            if(result)
            {
                List<Booking> allBookings = GetAllBookingsForEmployeeId(newBooking.Employee_Id);
                List<Booking> Conflicts = allBookings.Where(b => ((newBooking.StartDate > b.start_date && newBooking.StartDate < b.end_date) || (newBooking.StartDate < b.start_date && newBooking.EndDate > b.start_date ))).ToList();
                result = Conflicts.Count() == 0;
                if(!result)
                {
                    validationMessages.Add("Booking requested starting on " + newBooking.StartDate.ToString("MM/dd/yyyy")+
                        " and ending on " + newBooking.EndDate.ToString("MM/dd/yyyy") + " has conflict with the following booking(s)!");
                    foreach(var conflict in Conflicts)
                    {
                        validationMessages.Add("Booking ID: " + conflict.booking_id + " starting on " + conflict.start_date.ToString("MM/dd/yyyy") +
                                               " ending on " + conflict.end_date.ToString("MM/dd/yyyy"));
                    }
                }
            }
            //Check if employee is booked for more than 16 hours (2 shifts) a day

            return result;
        }


        public List<Booking> GetAllBookingsForEmployeeId(int employeeId)
        {
            if(GetEmployeeById(employeeId) != null)
            {
                return _db.Bookings.Where(b => b.employee_id == employeeId).ToList();
            }
            return null;
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
            return _db.Employees.Take(100).ToList();
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