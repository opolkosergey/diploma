using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Diploma.Core.Models;
using Diploma.Core.Services;
using Diploma.Options;
using Diploma.ViewModels;

namespace Diploma.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly DocumentService _documentService;
        private readonly SearchService _searchService;
        private readonly UserService _userService;

        public DocumentController(
            SearchService searchService,
            UserService userService, 
            DocumentService documentService)
        {
            _searchService = searchService;
            _userService = userService;
            _documentService = documentService;
        }

        [HttpPost]        
        public async Task<IActionResult> Upload(
            [FromServices] IOptions<UsersOptions> options, 
            IFormFileCollection filesCollection, 
            string folderName = "Uploaded")
        {         
            var file = filesCollection.FirstOrDefault();
            if (file == null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { messsage = "There is no file to save. Please select file and try again." });
            }

            if (options.Value.SupportedDocumentTypes.Contains(file.ContentType))
            {
                var user = await _userService.GetUserByEmail(User.Identity.Name);
                var document = await _documentService.Save(file, folderName, user);
                return RedirectToAction(nameof(HomeController.Index), "Home", new { folderId = document.DocumentFolders.First().UserFolderId });                
            }

            return RedirectToAction(nameof(HomeController.Error), "Home", new { messsage = "Document has unsupported content type." });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var user = await _userService.GetUserByEmail(User.Identity.Name);

            var result = await _documentService.Get(user, id);

            if (result == null)
            {
                return NotFound($"Document with id '{id}' was not found.");
            }

            return File(result.Content, result.ContentType);
        }        

        [HttpPost]
        public async Task<JsonResult> UserSearch([FromBody]SearchRequestModel model)
        {
            IEnumerable<ApplicationUser> users;

            if (model.MakeSearchOnlyInOrganization)
            {
                var user = await _userService.GetUserByEmail(User.Identity.Name);
                users = _searchService.SearchUsers(model.Username, user.OrganizationId);
            }
            else
            {
                users = _searchService.SearchUsers(model.Username);
            }

            return Json(users.Select(x => x.Email));
        }

        [HttpGet]
        public async Task<IActionResult> SignDocument([FromServices] DocumentSignService documentSignService, int id)
        {
            var user = await _userService.GetUserByEmail(User.Identity.Name);
            var document = await _documentService.Get(user, id);

            if (document == null)
            {
                return NotFound($"Document with id '{id}' was not found.");
            }

            if (await documentSignService.SignDocument(document, user))
            {
                await _documentService.Update(document);
                return RedirectToAction(nameof(HomeController.GetSignatureRequests), "Home");
            }

            return RedirectToAction(nameof(HomeController.Error), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DocumentDetails(int id)
        {
            var user = await _userService.GetUser(User);
            var document = await _documentService.Get(user, id);

            return View(document);
        }

        [HttpGet]
        public async Task<IActionResult> DocumentManage(int id)
        {
            var user = await _userService.GetUser(User);
            var document = await _documentService.Get(user, id);

            return View(new DocumentManageModel
            {
                Id = document.Id,
                DocumentName = document.DocumentName,
                Version = document.Version,
                UsersWithAccess = document.DocumentAccesses.Select(x => x.User).Distinct()
            });
        }

        [HttpPost]
        public async Task<IActionResult> DocumentManage(
            [FromServices] SignatureRequestService signatureRequestService, 
            DocumentManageModel documentModel)
        {
            var user = await _userService.GetUserByEmail(documentModel.NewAccessForUser);

            if (user == null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home", new { message = $"User with email '{documentModel.NewAccessForUser}' is not found." });
            }

            await _documentService.AddAccessForUser(user, documentModel.Id);

            if (documentModel.RequestSignature)
            {
                await signatureRequestService.CreateSignatureRequest(new IncomingSignatureRequest
                {
                    DocumentId = documentModel.Id,
                    UserRequester = User.Identity.Name,
                    ApplicationUserId = user.Id
                });
            }

            return RedirectToAction(nameof(DocumentManage), documentModel.Id);
        }

        public class SearchRequestModel
        {
            public string Username { get; set; }
            public bool MakeSearchOnlyInOrganization { get; set; }
        }
    }
}
