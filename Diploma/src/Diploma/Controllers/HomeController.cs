using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Services;
using Diploma.Helpers;
using Microsoft.AspNetCore.Mvc;
using Diploma.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Diploma.Options;
using Diploma.Models;

namespace Diploma.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DocumentService _documentService;
        private readonly OrganizationService _organizationService;
        private readonly SignatureRequestService _signatureRequestService;
        private readonly UserTaskService _userTaskService;        
        private readonly UserService _userService;

        public HomeController(
            DocumentService documentService, 
            OrganizationService organizationService, 
            UserTaskService userTaskService, 
            UserService userService, 
            SignatureRequestService signatureRequestService)
        {
            _documentService = documentService;
            _organizationService = organizationService;
            _userTaskService = userTaskService;
            _userService = userService;
            _signatureRequestService = signatureRequestService;
        }                       

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            [FromServices] IOptions<UsersOptions> options,
            [FromServices] SearchService searchService,
            int? folderId,
            string sortOrder, 
            string searchString, 
            int? page, 
            bool loginAsAnonimous)
        {
            SetViewData(sortOrder, searchString, loginAsAnonimous);

            if (searchString != null)
            {
                page = 1;
            }

            var model = new FolderViewModel();
            IEnumerable<Document> documents;
            if (folderId.HasValue)
            {
                var user = await _userService.GetUser(User);
                var folder = await _documentService.GetFolder(folderId.Value, user);
                model.FolderName = folder.Name;
                ViewData["FolderId"] = folderId;
                documents = folder.DocumentFolders.Select(x => x.Document);
            }
            else
            {
                if (loginAsAnonimous)
                {
                    documents = FakeDataGenerator.GenerateDocuments(15);
                }
                else
                {
                    documents = (await _documentService.GetAll())
                        .Where(x => x.DocumentAccesses.Any(u => u.User == User.Identity.Name))
                        .ToList();
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = searchService.SearchDocuments(searchString)
                    .Where(x => x.DocumentAccesses.Any(u => u.User == User.Identity.Name))
                    .ToList();
            }

            documents = SortDocuments(sortOrder, documents);

            model.Documents = PaginatedList<Document>.Create(documents, page ?? 1, options.Value.DocumentsCountOnPage);

            return View(model);
        }

        public async Task<IActionResult> UpdateDocumentInFolder(int documentId, int newFolderId)
        {
            var user = await _userService.GetUserByEmail(User.Identity.Name);
            await _documentService.UpdateDocumentLocation(documentId, newFolderId, user);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetSignatureRequests()
        {
            var user = await _userService.GetUser(User);
            var signatureRequests = _signatureRequestService.GetSignatureRequestsForUser(user);

            return View(signatureRequests.ToList());
        }

        [HttpGet]
        public IActionResult CreateSignatureWarrant() => View();        

        [HttpPost]
        public async Task<IActionResult> CreateSignatureWarrant(SignatureWarrant signatureWarrant)
        {
            var user = await _userService.GetUser(User);
            signatureWarrant.ApplicationUserId = user.Id;

            await _documentService.CreateSignatureWarrant(signatureWarrant);

            var warrantUser = await _userService.GetUserByEmail(signatureWarrant.ToUser);
            await _signatureRequestService.CloneSignatureRequests(user, warrantUser);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateUserTask() => View();        

        [HttpPost]
        public async Task<IActionResult> CreateUserTask(UserTask userTask)
        {
            userTask.Creator = User.Identity.Name;
            await _userTaskService.CreateTask(userTask);

            return RedirectToAction(nameof(MyTasks));
        }

        [HttpGet]
        public async Task<IActionResult> TasksList()
        {
            var user = await _userService.GetUserByEmail(User.Identity.Name);
            var tasks = await _userTaskService.GetTasksForOrganization(user.OrganizationId.Value);

            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> MyTasks()
        {
            var user = await _userService.GetUser(User);
            var tasks = _userTaskService.GetUserTasks(user.Email);

            return View(nameof(TasksList), tasks);
        }

        [AllowAnonymous]
        public IActionResult Error(string message = "Unknown error occured.") => View((object)message);        

        private IEnumerable<Document> SortDocuments(string sortOrder, IEnumerable<Document> documents)
        {
            switch (sortOrder)
            {
                case "name_desc": return documents.OrderByDescending(s => s.DocumentName);
                case "Date": return documents.OrderBy(s => s.UploadedDate);
                case "date_desc": return documents.OrderByDescending(s => s.UploadedDate);
                case "Version": return documents.OrderBy(s => s.Version);
                case "version_desc": return documents.OrderByDescending(s => s.Version);
                default: return documents.OrderBy(s => s.DocumentName);
            }
        }

        private void SetViewData(string sortOrder, string searchString, bool loginAsAnonimous)
        {
            ViewData["FolderId"] = null;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["VersionSortParm"] = sortOrder == "Version" ? "version_desc" : "Version";
            ViewData["anonimous"] = loginAsAnonimous;
            ViewData["CurrentFilter"] = searchString;
        }
    }
}
