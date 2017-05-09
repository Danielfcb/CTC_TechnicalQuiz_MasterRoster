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

        public ActionResult BookingsTable()
        {
            List<BookingCellValidationViewModel> weeklyBookingValidationResults = new List<BookingCellValidationViewModel>();
            DateTime startDate = new DateTime();

            string bookingPeriod = "Weekly";
            startDate = DateTime.Today; 
            
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["startDate"]))
                {
                    startDate = Convert.ToDateTime(Request.QueryString["startDate"]);                   
                }
                if(!string.IsNullOrEmpty(Request.QueryString["bookingPeriod"]))
                {
                    bookingPeriod = Request.QueryString["bookingPeriod"].ToString();
                }
                weeklyBookingValidationResults = _manager.GetBookingCellResults(startDate,bookingPeriod);
            }
            catch
            {
                bookingPeriod = "Weekly";
                weeklyBookingValidationResults = _manager.GetBookingCellResults(DateTime.Today, bookingPeriod);
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
            return View(_manager.CreateBookingAddForm());
        }

        //
        // POST: /Bookings/Create

        [HttpPost]
        public ActionResult Create(BookingAdd form)
        {
            try
            {
                if(form != null)
                {
                    List<string> validationMessage;
                    if(!_manager.InsertBookingToDatabase(form, out validationMessage))
                    {
                        // Re-populate booking form with validation message
                        BookingAddForm repopulatedForm = _manager.CreateBookingAddForm();
                        repopulatedForm.StartDate = form.StartDate;
                        repopulatedForm.EndDate = form.EndDate;
                        repopulatedForm.Comment = form.Comment;
                        repopulatedForm.Employees = new SelectList(_manager.GetAllEmployees(), "employee_id", "name", form.Employee_Id);
                        repopulatedForm.ValidationMessages = validationMessage;
                        repopulatedForm.Comment = form.Comment;
                        return View(repopulatedForm);
                    }
                    else
                    {
                        return RedirectToAction("BookingsTable");
                    }
                }
                else
                {
                    return RedirectToAction("BookingsTable");
                }
                
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Edit()
        {
            try
            {
                if (Request.QueryString["bookingId"] != null)
                {
                    int bookingId = Convert.ToInt32(Request.QueryString["bookingId"]);
                    BookingEditForm form =  _manager.CreateBookingEditForm(bookingId);

                    if(form != null)
                    {
                        return View(form);
                    }
                    else
                    {
                        return RedirectToAction("Bookings");
                    }
                }
            }
            catch
            {
                return RedirectToAction("Bookings");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Edit(BookingEdit form)
        {
            try
            {
                if(form != null)
                {
                    List<string> validationMessages;
                    if(_manager.UpdateBooking(form, out validationMessages))
                    {
                        return RedirectToAction("BookingsTable");
                    }
                    else
                    {
                        // Re-populate booking form with validation message
                        BookingEditForm repopulatedForm = _manager.CreateBookingEditForm(form.EmployeeId);
                        repopulatedForm.StartDate = form.StartDate;
                        repopulatedForm.EndDate = form.EndDate;
                        repopulatedForm.Comment = form.Comment;
                        repopulatedForm.ValidationMessages = validationMessages;
                        repopulatedForm.EmployeeId = form.EmployeeId;
                        repopulatedForm.EmployeeName = form.EmployeeName;
                        repopulatedForm.EmployeeNumber = form.EmployeeNumber;
                        repopulatedForm.Comment = form.Comment;
                        repopulatedForm.BookingType = new SelectList(_manager.GetAllBookingTypes(), "booking_type_code", "booking_type_name");
                        return View(repopulatedForm);
                    }
                }
                // TODO: Add update logic here

                return RedirectToAction("BookingsTable");
            }
            catch
            {
                return RedirectToAction("BookingsTable");
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
