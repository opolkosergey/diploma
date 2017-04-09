using System;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Repositories;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentRepository _documentRepository = new DocumentRepository();

        public Task<FileContentResult> DownloadFile(int id, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task Save(IFormFile file, ApplicationUser user)
        {
            var document = new Document
            {
                ContentType = file.ContentType,
                DocumentName = file.FileName,
                Owner = user
            };

            using (var reader = new System.IO.BinaryReader(file.OpenReadStream()))
            {
                document.Content = reader.ReadBytes((int)file.Length);
            }

            await _documentRepository.Save(document, user);
        }
    }
}
