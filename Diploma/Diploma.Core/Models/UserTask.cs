using System.ComponentModel.DataAnnotations;

namespace Diploma.Core.Models
{
    public class UserTask
    {
        public int Id { get; set; }

        [Required]
        public string Summary { get; set; }

        public string Description { get; set; }

        public bool SignatureRequired { get; set; }

        public string SignatureFromUser { get; set; }

        [Required]
        public string Creator { get; set; }

        [Required]
        public string AssignedTo { get; set; }

        public TaskStatus TaskStatus { get; set; }
    }
}
