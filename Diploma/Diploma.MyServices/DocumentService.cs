using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Update(Document document)
        {
            await _documentRepository.Update(document);
        }

        public async Task<FileContentResult> DownloadFile(int id, ApplicationUser user)
        {
            var document = await _documentRepository.Get(user, id);

            var result = new FileContentResult(document.Content, document.ContentType);

            result.FileDownloadName = CreateDocumentNameUsingVersion(document);

            return result;
        }

        public async Task<Document> Get(ApplicationUser user, int id)
        {
            return await _documentRepository.Get(user, id);
        }

        public async Task<IEnumerable<Document>> GetAll()
        {
            return await _documentRepository.GetAll();
        }

        public async Task Save(IFormFile file, string folderName, ApplicationUser user)
        {
            var folder = user.UserFolders.FirstOrDefault(x => x.Name == folderName);

            if (folder == null)
            {
                folder = new UserFolder
                {
                    Name = folderName,
                    ApplicationUser = user,
                    Documents = new List<Document>()
                };

                user.UserFolders.Add(folder);
            }

            var documentName = ParseFileName(file.FileName);

            var document = new Document
            {
                ContentType = file.ContentType,
                DocumentName = documentName,
                UserFolderId = folder.Id,
                UploadedDate = DateTime.Now,
                Version = await SetDocumentVersion(documentName),
                Size = file.Length,
                DocumentAccesses = new List<DocumentAccess>
                {
                    new DocumentAccess
                    {
                        User = user.Email
                    }
                },
            };

            using (var reader = new System.IO.BinaryReader(file.OpenReadStream()))
            {
                document.Content = reader.ReadBytes((int)file.Length);
            }

            await _documentRepository.Save(document, user);
        }

        private string ParseFileName(string fileName)
        {
            var pointIndex = fileName.LastIndexOf('.');

            var underscoreIndex = fileName.LastIndexOf('_');

            if (underscoreIndex != -1)
            {
                var version = fileName.Substring(underscoreIndex, pointIndex - underscoreIndex);

                return fileName.Replace(version, string.Empty);
            }

            return fileName;
        }

        private async Task<string> SetDocumentVersion(string fileFileName)
        {
            var documents = await _documentRepository.GetAll();

            var existingDocuments = documents
                .Where(x => x.DocumentName == fileFileName && !string.IsNullOrEmpty(x.Version))
                .ToList();

            if (existingDocuments.Any())
            {
                var versions = existingDocuments.Select(x => int.Parse(x.Version))
                    .OrderBy(i => i);

                var lastLoadedVersion = versions.Last();

                var version = lastLoadedVersion + 1;

                return version.ToString();
            }
            else
            {
                return "1";
            }
        }

        private string CreateDocumentNameUsingVersion(Document document)
        {
            if (string.IsNullOrEmpty(document.Version))
            {
                return document.DocumentName;
            }
            else
            {
                var index = document.DocumentName.LastIndexOf('.');
            
                var name = document.DocumentName.Insert(index, $"_{document.Version}");

                return name;
            }
        }
    }
}