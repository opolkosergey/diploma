using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Data;
using Diploma.Core.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers
{
    public class DocumentController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private readonly DocumentService _documentService = new DocumentService();

        public DocumentController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFileCollection filesCollection)
        {
            var ctx = new ApplicationDbContext();

            var file = filesCollection.First();

            var person = ctx.Users.First(x => x.Email == User.Identity.Name);

            await _documentService.Save(file, person);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            var ctx = new ApplicationDbContext();

            var document = ctx.Documents.First(x => x.Id == 2);

            var result = new FileContentResult(document.Content, document.ContentType);
            result.FileDownloadName = "Name.docx";

            return result;
        }
    }
}
