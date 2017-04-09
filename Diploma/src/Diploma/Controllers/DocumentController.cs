using System.Linq;
using Diploma.Data;
using Diploma.Models;
using Diploma.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers
{
    public class DocumentController : Controller
    {
        private DocumentService _documentService = new DocumentService();

        [HttpPost]
        public IActionResult Upload(IFormFileCollection img)
        {
            var ctx = new ApplicationDbContext();

            var file = img.First();

            var document = new Document
            {
                ContentType = file.ContentType,
                DocumentName = file.FileName,
            };

            using (var reader = new System.IO.BinaryReader(file.OpenReadStream()))
            {
                document.Content = reader.ReadBytes((int)file.Length);
            }

            ctx.Documents.Add(document);
            ctx.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
