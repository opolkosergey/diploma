using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Diploma.Core.Models;

namespace Diploma.Repositories.Abstracts
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAll(); 

        Task Save(Document document, ApplicationUser user);

        Task Update(Document document);

        Task<Document> Get(ApplicationUser user, int documentId);

        IEnumerable<Document> FindBy(Func<Document, bool> func);
    }
}
