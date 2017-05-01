using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Diploma.ViewModels
{
    public class DocumentManageModel
    {
        public int Id { get; set; }

        public string DocumentName { get; set; }

        public string Version { get; set; }

        [Required]
        public string NewAccessForUser { get; set; }

        public bool RequestSignature { get; set; }

        public IEnumerable<string> UsersWithAccess { get; set; }
    }
}
