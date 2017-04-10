using System;
using System.Collections.Generic;
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

        public async Task<FileContentResult> DownloadFile(int id, ApplicationUser user)
        {
            var document = await _documentRepository.Get(user, id);

            var result = new FileContentResult(document.Content, document.ContentType);

            result.FileDownloadName = CreateDocumentNameUsingVersion(document);

            result.FileDownloadName = document.DocumentName;

            return result;
        }

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await _documentRepository.GetAll();
        }

        public async Task Save(IFormFile file, ApplicationUser user)
        {
            var document = new Document
            {
                ContentType = file.ContentType,
                DocumentName = file.FileName,
                ApplicationUser = user,
                UploadedDate = DateTime.Now,
                Version = "1",
                DocumentAccesses = new List<DocumentAccess>
                {
                    new DocumentAccess
                    {
                        User = user.Email
                    }
                }
            };

            using (var reader = new System.IO.BinaryReader(file.OpenReadStream()))
            {
                document.Content = reader.ReadBytes((int)file.Length);
            }

            await _documentRepository.Save(document, user);
        }

        private string CreateDocumentNameUsingVersion(Document document)
        {
            return null;
        }
    }
}
