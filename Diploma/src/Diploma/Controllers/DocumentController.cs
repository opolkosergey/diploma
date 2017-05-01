﻿using System;
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
        private UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly DocumentService _documentService = new DocumentService();

        private readonly DocumentSignService _documentSignService = new DocumentSignService();

        private readonly SearchService _searchService;

        private readonly UserService _userService;

        public DocumentController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = new UserService(userManager, roleManager);
            _searchService = new SearchService(userManager);
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
        public async Task<FileResult> DownloadFile(int id)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            var result = await _documentService.DownloadFile(id, user);

            return result;
        }        

        [HttpPost]
        public JsonResult UserSearch([FromBody]SearchRequestModel model)
        {
            var users = new List<ApplicationUser>();

            if (model.OnlyInOrganization)
            {
                var user = _userService.GetUserByEmail(User.Identity.Name);

                users = _searchService.SearchUsers(model.Username, user.OrganizationId).ToList();
            }
            else
            {
                users = _searchService.SearchUsers(model.Username, null).ToList();
            }

            return Json(users.Select(x => new { label = x.Email }));
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

        public class SearchRequestModel
        {
            public string Username { get; set; }

            public bool OnlyInOrganization { get; set; }
        }
    }
}
