using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diploma.DocumentSign;
using Microsoft.AspNetCore.Authorization;

namespace Diploma.Controllers
{
    public class DocumentController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private readonly DocumentService _documentService = new DocumentService();

        private readonly DocumentSignService _documentSignService = new DocumentSignService();

        private readonly UserService _userService;

        public DocumentController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userService = new UserService(userManager);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFileCollection filesCollection, string folderName = "Uploaded")
        {         
            var file = filesCollection.First();

            var user = _userService.GetUserByEmail(User.Identity.Name);

            await _documentService.Save(file, folderName, user);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var result = await _documentService.DownloadFile(id, user);

            return result;
        }

        public async Task<IActionResult> SignDocument(int id)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var document = await _documentService.Get(user, id);

            if (_documentSignService.SignData(document, user))
            {
                await _documentService.Update(document);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
