using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Diploma.Pagging;
using Diploma.Services;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        private readonly DocumentService _documentService = new DocumentService();

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["VersionSortParm"] = sortOrder == "Version" ? "version_desc" : "Version";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var documents = (await _documentService.GetAll()).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(s => s.DocumentName.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    documents = documents.OrderByDescending(s => s.DocumentName).ToList();
                    break;
                case "Date":
                    documents = documents.OrderBy(s => s.UploadedDate).ToList();
                    break;
                case "date_desc":
                    documents = documents.OrderByDescending(s => s.UploadedDate).ToList();
                    break;
                case "Version":
                    documents = documents.OrderBy(s => s.Version).ToList();
                    break;
                case "version_desc":
                    documents = documents.OrderByDescending(s => s.Version).ToList();
                    break;
                default:
                    documents = documents.OrderBy(s => s.DocumentName).ToList();
                    break;
            }
            int pageSize = 10;

            return View(PaginatedList<Document>.CreateAsync(documents, page ?? 1, pageSize));
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
