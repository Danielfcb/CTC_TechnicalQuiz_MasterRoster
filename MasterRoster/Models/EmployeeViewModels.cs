using MasterRoster.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterRoster.Models
{
    public class EmployeeViewModels
    {
        public class AllEmployeesViewModel
        {
            public List<Employee> Employees { get; set; }
            public string Crew { get; set; }
            public string Role { get; set; }
            public SelectList AllRoles { get;set;}
            public SelectList AllCrews { get; set; }
        }
    }
}