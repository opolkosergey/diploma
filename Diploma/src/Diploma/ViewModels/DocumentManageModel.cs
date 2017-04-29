using System.Collections.Generic;

namespace Diploma.ViewModels
{
    public class DocumentManageModel
    {
        public int Id { get; set; }

        public string DocumentName { get; set; }

        public string Version { get; set; }

        public string NewAccessForUser { get; set; }

        public IEnumerable<string> UsersWithAccess { get; set; }
    }
}
