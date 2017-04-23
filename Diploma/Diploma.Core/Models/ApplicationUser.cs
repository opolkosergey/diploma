﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Diploma.Core.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool? IsOrganizationOwner { get; set; }
        
        public int? OrganizationId { get; set; }
        
        public virtual Organization Organization { get; set; }

        public virtual ICollection<UserFolder> UserFolders { get; set; }

        public virtual UserKeys UserKeys { get; set; }
    }
}
