using System;
using System.Collections.Generic;
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

        public double Size { get; set; }

        public DateTime UploadedDate { get; set; }

        public string Version { get; set; }

        public string SignedByUser { get; set; }

        public string Signature { get; set; }

        public virtual ICollection<DocumentAccess> DocumentAccesses { get; set; }

        public virtual ICollection<DocumentFolder> DocumentFolders { get; set; }              
    }
}