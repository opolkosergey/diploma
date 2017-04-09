using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Diploma.Models;
using Diploma.Pagging;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var students = new List<Student>
            {
                new Student
                {
                    EnrollmentDate = "1",
                    LastName = "AA"
                },
                new Student
                {
                    EnrollmentDate = "2",
                    LastName = "B"
                }
            };

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName).ToList();
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate).ToList();
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate).ToList();
                    break;
                default:
                    students = students.OrderBy(s => s.LastName).ToList();
                    break;
            }
            int pageSize = 3;
            return View(PaginatedList<Student>.CreateAsync(students.AsQueryable(), page ?? 1, 1));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
