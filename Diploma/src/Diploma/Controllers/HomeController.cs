using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Services;
using Diploma.Helpers;
using Microsoft.AspNetCore.Mvc;
using Diploma.Pagging;
using Diploma.ViewModels;
using Diploma.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        private readonly DocumentService _documentService;
        private readonly OrganizationService _organizationService;
        private readonly SignatureRequestService _signatureRequestService;
        private readonly UserTaskService _userTaskService;        
        private readonly UserService _userService;
        private readonly SearchService _searchService;

        private readonly UserManager<ApplicationUser> _userManager;        
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            DocumentService documentService, 
            OrganizationService organizationService, 
            UserTaskService userTaskService, 
            UserService userService, 
            SignatureRequestService signatureRequestService, 
            SearchService searchService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _documentService = documentService;
            _organizationService = organizationService;
            _userTaskService = userTaskService;
            _userService = userService;
            _signatureRequestService = signatureRequestService;
            _searchService = searchService;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> UpdateUser(string email, string role, string organization)
        {
            var org = _organizationService.GetOrganizationByName(organization);

            await _userService.UpdateUserByAdmin(email, role, org);

            await _documentService.UpdateUserAccesses(_userService.GetUserByEmail(email));

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

            ViewData["organizations"] = (await _organizationService.GetAll()).Select(x => x.Name);

            AddUserFolderToResponse(User.Identity.Name);

            return View(PaginatedList<AdminUserModel>.CreateAsync(users.ToList(),  1, 10));
        }

        public async Task<IActionResult> UpdateDocumentInFolder(int documentId, int newFolderId)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            await _documentService.UpdateDocumentLocation(documentId, newFolderId, user);

            return RedirectToAction("Index");
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

                await _signatureRequestService.CreateSignatureRequest(new IncomingSignatureRequest
                {
                    DocumentId = documentModel.Id,
                    UserRequester = User.Identity.Name,
                    ApplicationUserId = targetUser.Id
                });
            }

            AddUserFolderToResponse(User.Identity.Name);

            return RedirectToAction("DocumentManage", documentModel.Id);
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? page, bool loginAsAnonimous)
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

            ViewData["CurrentFilter"] = searchString;

            List<Document> documents;
            if (loginAsAnonimous)
            {
                documents = FictitiousDataGenerator.GeneratDocuments();
            }
            else
            {
                documents = (await _documentService.GetAll())
                    .Where(x => x.DocumentAccesses.Any(u => u.User == User.Identity.Name))
                    .ToList();
            }           

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = (await _searchService.SearchDocuments(searchString))
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
        public IActionResult CreateNewFolder()
        {
            AddUserFolderToResponse(User.Identity.Name);

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateNewFolder(UserFolder folder)
        {
            AddUserFolderToResponse(User.Identity.Name);

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFolderContent(int folderId)
        {
            AddUserFolderToResponse(User.Identity.Name);

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSignatureRequests()
        {
            var signatureRequests = await _signatureRequestService.GetSignatureRequestsForUser(_userService.GetUserByEmail(User.Identity.Name));
            AddUserFolderToResponse(User.Identity.Name);

            return View(signatureRequests.ToList());
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

            await _documentService.CreateSignatureWarrant(signatureWarrant);

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

        [Route("/Home/Error")]
        public IActionResult Error()
        {
            object message = "Unknown error occured.";

            return View(message);
        }
    }
}
