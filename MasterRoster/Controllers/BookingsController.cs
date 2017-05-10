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
            List<BookingCellValidationViewModel> model = new List<BookingCellValidationViewModel>();
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
                model = _manager.GetBookingCellResults(startDate,bookingPeriod);
            }
            catch
            {
                bookingPeriod = "Weekly";
                model = _manager.GetBookingCellResults(DateTime.Today, bookingPeriod);
            }

            return View(model);
        }


        public ActionResult Details()
        {
            // TODO: validation logic here for querystring
            var crew = Request.QueryString["Crew"];
            var date = Convert.ToDateTime(Request.QueryString["Date"]);
            var bookingCellInfo = _manager.GetBookingCellInfo(crew,date);
            return View(bookingCellInfo);
        }


        public ActionResult Create()
        {
            return View(_manager.CreateBookingAddForm());
        }


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
                        return View(_manager.RepopulateCreateForm(form, validationMessage));
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
                return View(_manager.CreateBookingAddForm());
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
                        return View(_manager.RepopulateEditForm(form, validationMessages));
                    }
                }
                return RedirectToAction("BookingsTable");
            }
            catch
            {
                return RedirectToAction("BookingsTable");
            }
        }

        //TODO: Add delete functionality

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
