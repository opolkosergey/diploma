using System.Collections.Generic;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Services.Abstracts
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAll();
        
        Task Save(IFormFile file, ApplicationUser user);

        Task<FileContentResult> DownloadFile(int id, ApplicationUser user);
    }
}
