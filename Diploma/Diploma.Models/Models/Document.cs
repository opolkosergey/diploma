using System.ComponentModel.DataAnnotations;

namespace Diploma.Core.Models
{
    public class Document
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string DocumentName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public virtual ApplicationUser Person { get; set; }
    }
}
