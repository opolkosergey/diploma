using System.Collections.Generic;

namespace Diploma.Core.Models
{
    public class Organization
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public virtual ICollection<ApplicationUser> Employees { get; set; }
    }
}
