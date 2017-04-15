using System;
using System.Collections.Generic;
using System.Text;

namespace Diploma.Core.Models
{
    public class UserFolder
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}
