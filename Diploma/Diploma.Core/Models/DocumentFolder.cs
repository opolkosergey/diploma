using System.ComponentModel.DataAnnotations.Schema;

namespace Diploma.Core.Models
{
    public class DocumentFolder
    {
        [ForeignKey("UserFolder")]
        public int UserFolderId { get; set; }

        public virtual UserFolder UserFolder { get; set; }

        [ForeignKey("Document")]
        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }
    }
}
