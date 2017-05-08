using MasterRoster.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MasterRoster.Models.BookingViewModels;

namespace MasterRoster.Controllers
{
    public class BookingsController : Controller
    {

        private Manager _manager = new Manager();
        //public ActionResult Index()
        //{
            //var currentWeeksBooking = _manager.GetCurrentWeeksBooking();

            //_manager.GetWeeklyBookingCellResults(DateTime.Today);

            //return View("Index2", _manager.GetCurrentWeeksBooking());

            //var weeklyBookingValidationResults = _manager.GetWeeklyBookingCellResults(DateTime.Today);
            //return View(weeklyBookingValidationResults);
        //}

        public ActionResult WeeklyBookings()
        {
            List<BookingCellValidationViewModel> weeklyBookingValidationResults = new List<BookingCellValidationViewModel>();
            DateTime startDate = new DateTime();
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["startDate"]))
                {
                    startDate = Convert.ToDateTime(Request.QueryString["startDate"]);                   
                }
                else
                {
                    startDate = DateTime.Today;
                }
                weeklyBookingValidationResults = _manager.GetWeeklyBookingCellResults(startDate);
            }
            catch
            {
                weeklyBookingValidationResults = _manager.GetWeeklyBookingCellResults(DateTime.Today);
            }

            return View(weeklyBookingValidationResults);
        }

        //
        // GET: /Bookings/Details/5

        public ActionResult Details()
        {
            var crew = Request.QueryString["Crew"];
            var date = Convert.ToDateTime(Request.QueryString["Date"]);
            var bookingCellInfo = _manager.GetBookingCellInfo(crew,date);
            return View(bookingCellInfo);
        }

        //
        // GET: /Bookings/Create

        public ActionResult Create()
        {
            var allEmployees = _manager.GetAllEmployees();
            var allBookingTypes = _manager.GetAllBookingTypes();

            var bookingAddForm = new BookingAddForm()
            {
                BookingTypes = new SelectList(allBookingTypes , "booking_type_code", "booking_type_name"),
                Employees = new SelectList(allEmployees, "employee_id", "name")
            };

            return View(bookingAddForm);
        }

        //
        // POST: /Bookings/Create

        [HttpPost]
        public ActionResult Create(BookingAdd form)
        {
            try
            {
                // TODO: Add insert logic here
                if(form != null)
                {
                    _manager.InsertBookingToDatabase(form);
                }
                return RedirectToAction("WeeklyBookings");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Bookings/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Bookings/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Bookings/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Bookings/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
