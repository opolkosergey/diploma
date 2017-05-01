using System;
using System.ComponentModel.DataAnnotations;

namespace Diploma.Core.Models
{
    public class SignatureWarrant
    {
        public int Id { get; set; }        

        [Required]
        public string ToUser { get; set; }

        [Required]
        public DateTime Expired { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
