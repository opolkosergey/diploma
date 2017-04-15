namespace Diploma.Core.Models
{
    public class DocumentAccess
    {
        public int Id { get; set; }

        public string User { get; set; }

        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }
    }
}
