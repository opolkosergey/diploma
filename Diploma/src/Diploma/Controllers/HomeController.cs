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
using Diploma.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        private readonly DocumentService _documentService = new DocumentService();
        private readonly OrganizationService organizationService = new OrganizationService();
        private readonly UserTaskService _userTaskService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = new UserService(userManager, roleManager);
            _userTaskService = new UserTaskService(userManager, roleManager);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> UpdateUser(string email, string role, string organization)
        {
            var org = await organizationService.GetOrganizationByName(organization);

            await _userService.UpdateUserByAdmin(email, role, org);

            return RedirectToAction("Users", "Home", new { forEdit = true });
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Users(int page = 1, bool forEdit = false)
        {
            ViewBag.ForEdit = forEdit;

            var users = _userManager.Users.Select(user => new AdminUserModel
            {
                Email = user.Email,
                Organization = user.Organization == null ? null : user.Organization.Name,
                Role = _roleManager.Roles.First(x => x.Id == user.Roles.First().RoleId).Name
            });//.Where(u => u.Role != "Administrator");

            ViewData["organizations"] = (await organizationService.GetAll()).Select(x => x.Name);

            AddUserFolderToResponse(User.Identity.Name);

            return View(PaginatedList<AdminUserModel>.CreateAsync(users.ToList(),  1, 10));
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

        public async Task<IActionResult> CreateUserTask(UserTask userTask)
        {
            await _userTaskService.CreateTask(userTask);

            return await TasksList(_userService.GetUserByEmail(userTask.AssignedTo).OrganizationId.Value);
        }

        public async Task<IActionResult> TasksList(int organizationId)
        {
            var tasksForOrganization = _userTaskService.GetTasksForOrganization(organizationId);

            return null;
        }

        public async Task<IActionResult> MyTasks()
        {
            var tasksForOrganization = _userTaskService.GetUserTasks(User.Identity.Name);

            return null;
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
