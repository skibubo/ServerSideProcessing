using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServerSideProcessing.Models;
using System.Linq.Dynamic;

namespace ServerSideProcessing.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetList()
        {
            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            List<Employee> empList = new List<Employee>();
            using (DBModel db = new DBModel())
            {
                empList = db.Employees.ToList<Employee>();
                int totalrows = empList.Count;
                if (!string.IsNullOrEmpty(searchValue))//filter
                {
                    empList = empList.
                        Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) || x.Position.ToLower().Contains(searchValue.ToLower()) || x.Office.ToLower().Contains(searchValue.ToLower()) || x.Age.ToString().Contains(searchValue.ToLower()) || x.Salary.ToString().Contains(searchValue.ToLower())).ToList<Employee>();
                }

                int totalrowsafterfiltering = empList.Count;
                //sorting
                empList = empList.OrderBy(sortColumnName + " " + sortDirection).ToList<Employee>();

                //paging
                empList = empList.Skip(start).Take(length).ToList<Employee>();

                return Json(new { data = empList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}