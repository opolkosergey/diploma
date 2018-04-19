using Diploma.Core.Models;
using Diploma.Pagging;

namespace Diploma.Models
{
    public class FolderViewModel
    {
        public string FolderName { get; set; }

        public PaginatedList<Document> Documents { get; set; }
    }
}
