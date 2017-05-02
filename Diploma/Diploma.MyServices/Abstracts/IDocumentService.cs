﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Services.Abstracts
{
    public interface IDocumentService
    {
        Task<Document> Get(ApplicationUser user, int id);

        Task<IEnumerable<Document>> GetAll();
        
        Task Save(IFormFile file, string filderName, ApplicationUser user);

        Task Update(Document document);

        Task AddAccessForUser(ApplicationUser user, int documentId);

        Task UpdateUserAccesses(ApplicationUser user);

        Task CreateNewFolder(UserFolder userFolder, ApplicationUser user);

        Task<FileContentResult> DownloadFile(int id, ApplicationUser user);
    }
}
