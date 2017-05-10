using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Diploma.DocumentSign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diploma.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DocumentService _documentService;

        private readonly DocumentSignService _documentSignService = new DocumentSignService();

        private readonly SearchService _searchService;

        private readonly UserService _userService;

        public DocumentController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userService = new UserService(userManager, roleManager);
            _searchService = new SearchService(userManager);
            _documentService = new DocumentService(userManager);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFileCollection filesCollection, string folderName = "Uploaded")
        {         
            var file = filesCollection.FirstOrDefault();

            if (file == null)
            {
                return View("Error", "There are no file to save. Please select file and try again.");
            }

            if (file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" 
                || file.ContentType == "application/msword")
            {
                var user = _userService.GetUserByEmail(User.Identity.Name);

                await _documentService.Save(file, folderName, user);

                return RedirectToAction("Index", "Home");                
            }

            return View("Error", "Invalid content type.");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var result = await _documentService.DownloadFile(id, user);

            if (result == null)
            {
                return new NotFoundResult();
            }

            return result;
        }        

        [HttpPost]
        public async Task<JsonResult> UserSearch([FromBody]SearchRequestModel model)
        {
            var users = new List<ApplicationUser>();

            if (model.OnlyInOrganization)
            {
                var user = _userService.GetUserByEmail(User.Identity.Name);

                users = (await _searchService.SearchUsers(model.Username, user.OrganizationId)).ToList();
            }
            else
            {
                users = (await _searchService.SearchUsers(model.Username, null)).ToList();
            }

            return Json(users.Select(x => new { label = x.Email }));
        }

        [Authorize]
        public async Task<IActionResult> SignDocument(int id)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var document = await _documentService.Get(user, id);

            if (document == null)
            {
                return new NotFoundResult();
            }

            if (await _documentSignService.SignDocument(document, user))
            {
                await _documentService.Update(document);
                return RedirectToAction("GetSignatureRequests", "Home");
            }

            return RedirectToAction("Error", "Home");
        }

        public class SearchRequestModel
        {
            public string Username { get; set; }

            public bool OnlyInOrganization { get; set; }
        }
    }
}
