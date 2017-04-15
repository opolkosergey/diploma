using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Controllers
{
    public class DocumentController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private readonly DocumentService _documentService = new DocumentService();

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
        public async Task<IActionResult> DownloadFile(int id)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var result = await _documentService.DownloadFile(id, user);

            return result;
        }
    }
}
