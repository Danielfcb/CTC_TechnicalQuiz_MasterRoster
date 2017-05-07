using MasterRoster.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterRoster.Controllers
{
    public class BookingsController : Controller
    {

        private Manager _manager = new Manager();
        public ActionResult Index()
        {
            var currentWeeksBooking = _manager.GetCurrentWeeksBooking();
            return View(currentWeeksBooking);
        }

        //
        // GET: /Bookings/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Bookings/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Bookings/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
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
