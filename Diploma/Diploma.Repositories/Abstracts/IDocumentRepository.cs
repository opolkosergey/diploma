using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Repositories.Abstracts
{
    public interface IDocumentRepository
    {
        Task Save(Document document, ApplicationUser user);

        Task<Document> Get(ApplicationUser user, int documentId);
    }
}
