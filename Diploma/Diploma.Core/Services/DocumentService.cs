using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Diploma.Core.Services
{
    public class DocumentService
    {
        private readonly BaseRepository<Document> _documentRepository;
        private readonly BaseRepository<UserFolder> _userFolderRepository;
        private readonly BaseRepository<SignatureWarrant> _signatureWarrantRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentService(
            UserManager<ApplicationUser> userManager, 
            BaseRepository<UserFolder> userFolderRepository, 
            BaseRepository<Document> documentRepository, 
            BaseRepository<SignatureWarrant> signatureWarrantRepository)
        {
            _userManager = userManager;            
            _userFolderRepository = userFolderRepository;
            _documentRepository = documentRepository;
            _signatureWarrantRepository = signatureWarrantRepository;
        }

        public async Task Update(Document document) => await _documentRepository.Update(document);        

        public async Task AddAccessForUser(ApplicationUser user, int documentId)
        {
            var document = await _documentRepository.Get(documentId.ToString());

            if (document.DocumentAccesses.SingleOrDefault(x => x.User == user.Email) == null)
            {
                document.DocumentAccesses.Add(new DocumentAccess { User = user.Email });
                await Update(document);
            }
        }

        public async Task CreateSignatureWarrant(SignatureWarrant signatureWarrant) => await _signatureWarrantRepository.Add(signatureWarrant);

        public async Task UpdateUserDocumentAccesses(ApplicationUser user, bool addDocumentAccessesInCurentOrganization = false)
        {
            var allDocuments = await _documentRepository.GetAll();

            var userDocuments = allDocuments.Where(x => x.DocumentAccesses.Any(a => a.User == user.Email)).ToList();

            foreach (var document in userDocuments)
            {
                var docAccess = document.DocumentAccesses.Single(x => x.User == user.Email);

                document.DocumentAccesses.Remove(docAccess);

                await _documentRepository.Update(document);
            }

            var usersFromNewOrganization = _userManager.Users
                .Where(x => x.OrganizationId.HasValue && x.OrganizationId == user.OrganizationId)
                .Select(u => u.Email)
                .ToList();

            var newDocumentsForUser = allDocuments.Where(x => x.DocumentAccesses.Any(u => usersFromNewOrganization.Contains(u.User)));
            var documentsIds = newDocumentsForUser.Select(x => x.Id).Distinct().ToList();
            documentsIds.ForEach(async documentId => await AddAccessForUser(user, documentId));
        }

        public async Task CreateNewFolder(UserFolder userFolder, ApplicationUser user)
        {
            userFolder.ApplicationUserId = user.Id;
            await _userFolderRepository.Add(userFolder);
        }

        public async Task<UserFolder> GetFolder(int userFolderId, ApplicationUser user)
        {
            var userFolder = await _userFolderRepository.Get(userFolderId.ToString());

            if (userFolder != null && userFolder.ApplicationUserId == user.Id)
            {
                return userFolder;
            }

            return null;
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

            await _userFolderRepository.Update(folder);

            var newFolder = user.UserFolders.FirstOrDefault(x => x.Id == newFolderId);

            if (newFolder == null)
            {
                return false;
            }

            newFolder.DocumentFolders.Add(new DocumentFolder{ DocumentId = documentId });

            await _userFolderRepository.Update(newFolder);

            return true;
        }

        public async Task<Document> Get(ApplicationUser user, int id)
        {
            var document = await _documentRepository.Get(id.ToString());

            if (document != null && document.DocumentAccesses.Any(x => x.User == user.Email))
            {
                return document;
            }

            return null;
        }

        public async Task<IEnumerable<Document>> GetAll() => await _documentRepository.GetAll();

        public string CreateDocumentNameUsingVersion(Document document)
        {
            if (string.IsNullOrEmpty(document.Version))
            {
                return document.DocumentName;
            }
            else
            {
                var index = document.DocumentName.LastIndexOf('.');

                var name = document.DocumentName.Insert(index, $"--{document.Version}");

                return name;
            }
        }

        public async Task<Document> Save(IFormFile file, string folderName, ApplicationUser user)
        {
            var document = await CreateDocument(file, user);

            using (var reader = new System.IO.BinaryReader(file.OpenReadStream()))
            {
                document.Content = reader.ReadBytes((int)file.Length);
            }

            var folder = CreateFolderIfNotExists(folderName, user);

            folder.DocumentFolders.Add(
                new DocumentFolder
                {
                    Document = document
                }
            );

            await _userFolderRepository.Update(folder);

            return document;
        }

        private async Task<Document> CreateDocument(IFormFile file, ApplicationUser user)
        {
            var documentName = ParseFileName(file.FileName);

            var document = new Document
            {
                ContentType = file.ContentType,
                DocumentName = documentName,
                UploadedDate = DateTime.Now,
                Version = (await SetDocumentVersion(documentName)).ToString(),
                Size = file.Length,
                DocumentAccesses = new List<DocumentAccess>
                {
                    new DocumentAccess
                    {
                        User = user.Email
                    }
                }
            };

            return document;
        }

        private static UserFolder CreateFolderIfNotExists(string folderName, ApplicationUser user)
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

            return folder;
        }

        private string ParseFileName(string fileName)
        {
            var pointIndex = fileName.LastIndexOf('.');

            var underscoreIndex = fileName.LastIndexOf("--", StringComparison.OrdinalIgnoreCase);

            if (underscoreIndex != -1)
            {
                var version = fileName.Substring(underscoreIndex, pointIndex - underscoreIndex);

                return fileName.Replace(version, string.Empty);
            }

            return fileName;
        }

        private async Task<int> SetDocumentVersion(string fileFileName)
        {
            var documents = await _documentRepository.GetAll();

            var existingDocuments = documents
                .Where(x => x.DocumentName == fileFileName && !string.IsNullOrEmpty(x.Version))
                .ToList();

            if (existingDocuments.Any())
            {
                var versions = existingDocuments.Select(x => int.Parse(x.Version)).OrderBy(i => i);
                return versions.Last() + 1;
            }
            else
            {
                return 1;
            }
        }
    }
}