using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Filters;
using Diploma.Helpers;
using Microsoft.AspNetCore.Mvc;
using Diploma.Pagging;
using Diploma.Services;
using Microsoft.AspNetCore.Identity;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        private readonly DocumentService _documentService = new DocumentService();
        private UserManager<ApplicationUser> _userManager;
        private readonly UserService _userService;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userService = new UserService(userManager);
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, bool loginAsAnonimous)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["VersionSortParm"] = sortOrder == "Version" ? "version_desc" : "Version";
            ViewData["anonimous"] = loginAsAnonimous;

            AddUserFolderToResponse(User.Identity.Name);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            List<Document> documents;
            if (loginAsAnonimous)
            {
                documents = new List<Document>
                {
                    new Document
                    {
                        DocumentName = "Example document",
                        Version = "1",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document",
                        Version = "2",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document",
                        Version = "1",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document1",
                        Version = "1",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document1",
                        Version = "2",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document1",
                        Version = "3",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document2",
                        Version = "1",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document2",
                        Version = "2",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document2",
                        Version = "3",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document2",
                        Version = "4",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document3",
                        Version = "1",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document3",
                        Version = "2",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document3",
                        Version = "3",
                        UploadedDate = DateTime.Now
                    },
                    new Document
                    {
                        DocumentName = "Example document3",
                        Version = "4",
                        Signature = "jisogj9reugr09u5y4ih90g",
                        SignedByUser = "Example user",
                        UploadedDate = DateTime.Now
                    }
                };
            }
            else
            {
                documents = (await _documentService.GetAll()).ToList();
            }           

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = new SearchService().SearchDocuments(searchString).ToList();
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

        private void AddUserFolderToResponse(string identityName)
        {
            if (string.IsNullOrEmpty(identityName))
            {
                ViewData["Folders"] = FictitiousDataGenerator.GenerateFolders();
            }
            else
            {
                ViewData["Folders"] = _userService.GetUserByEmail(User.Identity.Name).UserFolders.AsEnumerable();
            }
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
