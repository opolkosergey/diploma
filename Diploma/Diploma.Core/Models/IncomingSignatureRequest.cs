namespace Diploma.Core.Models
{
    public class IncomingSignatureRequest
    {
        public int Id { get; set; }

        public string UserRequester { get; set; }

        public int DocumentId { get; set; }

        public bool ClonnedUsingWarrant { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
