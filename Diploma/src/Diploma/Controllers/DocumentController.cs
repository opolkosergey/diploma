using System;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diploma.DocumentSign;
using Diploma.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diploma.Controllers
{
    public class DocumentController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly DocumentService _documentService = new DocumentService();

        private readonly DocumentSignService _documentSignService = new DocumentSignService();

        private readonly UserService _userService;

        public DocumentController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = new UserService(userManager, roleManager);
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

        public async Task<IActionResult> DocumentDetails(int id)
        {
            var document = await _documentService.Get(_userService.GetUserByEmail(User.Identity.Name), id);

            document.Size /= 1024;

            document.Size = Math.Ceiling(document.Size);

            return View(document);
        }

        [HttpGet]
        public async Task<IActionResult> DocumentManage(int id)
        {
            var document = await _documentService.Get(_userService.GetUserByEmail(User.Identity.Name), id);

            return View(new DocumentManageModel
            {
                Id = document.Id,
                DocumentName = document.DocumentName,
                Version = document.Version,
                UsersWithAccess = document.DocumentAccesses.Select(x => x.User)
            });
        }

        [HttpPost]
        public async Task<IActionResult> DocumentManage(DocumentManageModel document)
        {
            var user = _userService.GetUserByEmail(document.NewAccessForUser);

            if (user == null)
            {
                return View("Error", $"User with email '{document.NewAccessForUser}' is not found.");
            }

            await _documentService.AddAccessForUser(user, document.Id);

            return RedirectToAction("DocumentManage", document.Id);
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
