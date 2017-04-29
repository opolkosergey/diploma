using System;

namespace Diploma.Core.Models
{
    public class SignatureWarrant
    {
        public int Id { get; set; }

        public string FromUser { get; set; }

        public string ToUser { get; set; }

        public DateTime Expired { get; set; }
    }
}
