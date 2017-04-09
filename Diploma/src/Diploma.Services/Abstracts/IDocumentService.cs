using System;
using System.Collections.Generic;
using System.Text;
using Diploma.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Services.Abstracts
{
    public interface IDocumentService
    {
        void Save(IFormFile file, ApplicationUser user);

        FileContentResult DownloadFile(int id);
    }
}
