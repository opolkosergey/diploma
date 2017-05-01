using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Filters;
using Diploma.Helpers;
using Microsoft.AspNetCore.Mvc;
using Diploma.Pagging;
using Diploma.Repositories;
using Diploma.Services;
using Diploma.ViewModels;
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
        private readonly SignatureRequestRepository _signatureRequestRepository = new SignatureRequestRepository();
        private readonly SignatureRequestService _signatureRequestService = new SignatureRequestService();
        private readonly SignatureWarrantRepository _signatureWarrantService = new SignatureWarrantRepository();


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

        public async Task<IActionResult> DocumentDetails(int id)
        {
            var document = await _documentService.Get(_userService.GetUserByEmail(User.Identity.Name), id);

            document.Size /= 1024;

            document.Size = Math.Ceiling(document.Size);

            AddUserFolderToResponse(User.Identity.Name);

            return View(document);
        }

        [HttpGet]
        public async Task<IActionResult> DocumentManage(int id)
        {
            var document = await _documentService.Get(_userService.GetUserByEmail(User.Identity.Name), id);

            AddUserFolderToResponse(User.Identity.Name);

            return View(new DocumentManageModel
            {
                Id = document.Id,
                DocumentName = document.DocumentName,
                Version = document.Version,
                UsersWithAccess = document.DocumentAccesses.Select(x => x.User).Distinct()
            });
        }

        [HttpPost]
        public async Task<IActionResult> DocumentManage(DocumentManageModel documentModel)
        {
            var user = _userService.GetUserByEmail(documentModel.NewAccessForUser);

            if (user == null)
            {
                return View("Error", $"User with email '{documentModel.NewAccessForUser}' is not found.");
            }

            await _documentService.AddAccessForUser(user, documentModel.Id);

            if (documentModel.RequestSignature)
            {
                var targetUser = _userService.GetUserByEmail(documentModel.NewAccessForUser);

                await _signatureRequestRepository.Create(new IncomingSignatureRequest
                {
                    DocumentId = documentModel.Id,
                    UserRequester = User.Identity.Name,
                    ApplicationUserId = targetUser.Id
                });
            }

            AddUserFolderToResponse(User.Identity.Name);
            return RedirectToAction("DocumentManage", documentModel.Id);
        }

        public async Task<IActionResult> Index(string sortOrder, /*string currentFilter,*/ string searchString, int? page, bool loginAsAnonimous)
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
                //searchString = currentFilter;
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
                documents = (await _documentService.GetAll())
                    .Where(x => x.DocumentAccesses.Any(u => u.User == User.Identity.Name))
                    .ToList();
            }           

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = new SearchService(_userManager).SearchDocuments(searchString)
                    .Where(x => x.DocumentAccesses.Any(u => u.User == User.Identity.Name))
                    .ToList();
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

        [HttpGet]
        [Authorize]
        public IActionResult CreateSignatureWarrant()
        {
            AddUserFolderToResponse(User.Identity.Name);

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSignatureWarrant(SignatureWarrant signatureWarrant)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            signatureWarrant.ApplicationUserId = user.Id;

            await _signatureWarrantService.CreateSignatureWarrant(signatureWarrant);

            var warrantUser = _userService.GetUserByEmail(signatureWarrant.ToUser);

            await _signatureRequestService.CloneSignatureRequests(user, warrantUser);

            AddUserFolderToResponse(User.Identity.Name);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateUserTask()
        {
            AddUserFolderToResponse(User.Identity.Name);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserTask(UserTask userTask)
        {
            userTask.Creator = User.Identity.Name;

            await _userTaskService.CreateTask(userTask);

            AddUserFolderToResponse(User.Identity.Name);

            return RedirectToAction("TasksList");
        }

        [HttpGet]
        public IActionResult TasksList()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var tasks = _userTaskService.GetTasksForOrganization(user.OrganizationId.Value);

            AddUserFolderToResponse(User.Identity.Name);

            return View(tasks);
        }

        [HttpGet]
        public IActionResult MyTasks()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var tasks = _userTaskService.GetUserTasks(user.Email);

            AddUserFolderToResponse(User.Identity.Name);

            return View("TasksList", tasks);
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
