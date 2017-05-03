using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Repositories;
using Diploma.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DocumentRepository _documentRepository = new DocumentRepository();
        private readonly SignatureWarrantRepository _signatureWarrantRepository = new SignatureWarrantRepository();

        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Update(Document document)
        {
            await _documentRepository.Update(document);
        }

        public async Task AddAccessForUser(ApplicationUser user, int documentId)
        {
            var document = await _documentRepository.Get(documentId);

            document.DocumentAccesses.Add(new DocumentAccess
            {
                User = user.Email
            });

            await Update(document);
        }

        public async Task CreateSignatureWarrant(SignatureWarrant signatureWarrant)
        {
            await _signatureWarrantRepository.CreateSignatureWarrant(signatureWarrant);
        }

        public async Task UpdateUserAccesses(ApplicationUser user)
        {
            var allDocuments = await _documentRepository.GetAll();

            var userDocuments = allDocuments
                .Where(x => x.DocumentAccesses.Any(a => a.User == user.Email))
                .ToList();

            foreach (var document in userDocuments)
            {
                var docAccess = document.DocumentAccesses.Single(x => x.User == user.Email);

                document.DocumentAccesses.Remove(docAccess);

                await _documentRepository.Update(document);
            }

            var usersFormNewOrganization = _userManager.Users
                .Where(x => x.OrganizationId.HasValue && x.OrganizationId == user.OrganizationId)
                .Select(u => u.Email)
                .ToList();

            var newDocumentsForUser = allDocuments.Where(x => x.DocumentAccesses.Any(u => usersFormNewOrganization.Contains(u.User)));
            var documentsIds = newDocumentsForUser.Select(x => x.Id).Distinct();

            foreach (var documentId in documentsIds)
            {
                await AddAccessForUser(user, documentId);
            }
        }

        public async Task CreateNewFolder(UserFolder userFolder, ApplicationUser user)
        {
            user.UserFolders.Add(userFolder);

            await _documentRepository.SaveFolder(user);
        }

        public async Task<bool> UpdateDocumentLocation(int documentId, int newFolderId, ApplicationUser user)
        {
            var folder = user.UserFolders.FirstOrDefault(x => x.DocumentFolders.Any(d => d.DocumentId == documentId));
            if (folder == null)
            {
                return false;
            }

            var documentFolder = folder.DocumentFolders.FirstOrDefault(d => d.DocumentId == documentId);

            folder.DocumentFolders.Remove(documentFolder);

            await _documentRepository.Update(folder, user);

            var newFolder = user.UserFolders.FirstOrDefault(x => x.Id == newFolderId);

            if (newFolder == null)
            {
                return false;
            }

            newFolder.DocumentFolders.Add(
                new DocumentFolder
                {
                    DocumentId = documentId
                }
            );

            await _documentRepository.Update(newFolder, user);

            return true;
        }

        public async Task<FileContentResult> DownloadFile(int id, ApplicationUser user)
        {
            var document = await _documentRepository.Get(id);

            var result = new FileContentResult(document.Content, document.ContentType);

            result.FileDownloadName = CreateDocumentNameUsingVersion(document);

            return result;
        }

        public async Task<Document> Get(ApplicationUser user, int id)
        {
            var document = await _documentRepository.Get(id);

            if (document != null && document.DocumentAccesses.Any(x => x.User == user.Email))
            {
                return document;
            }
            else
            {
                return null;
            }
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
                    ApplicationUserId = user.Id
                };

                user.UserFolders.Add(folder);
            }

            var documentName = ParseFileName(file.FileName);

            var document = new Document
            {
                ContentType = file.ContentType,
                DocumentName = documentName,
                //UserFolders = new List<UserFolder>
                //{
                //    folder
                //},
                UploadedDate = DateTime.Now,
                Version = await SetDocumentVersion(documentName),
                Size = file.Length,
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

            folder.DocumentFolders.Add(
                new DocumentFolder
                {
                    Document = document
                }
            );

            await _documentRepository.Update(folder, user);
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