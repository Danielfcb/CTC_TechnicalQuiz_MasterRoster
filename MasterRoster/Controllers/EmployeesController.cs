using MasterRoster.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterRoster.Controllers
{
    public class EmployeesController : Controller
    {
        private Manager _manager = new Manager();
        public ActionResult Index()
        {
            string crew = "DAY";
            string role = "Manager";

            if (!string.IsNullOrEmpty(Request.QueryString["crew"]))
            {
                crew = Request.QueryString["crew"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["role"]))
            {
                role = Request.QueryString["role"].ToString();
            }
            var model = _manager.GetAllEmployees(crew, role);

            return View(model);
        }

        [HttpGet]
        public ActionResult Details()
        {
            int employeeId = Convert.ToInt32(Request.QueryString["employeeId"]);
            var employee = _manager.GetEmployeeById(employeeId);
            return View(employee);
        }
    }
}
