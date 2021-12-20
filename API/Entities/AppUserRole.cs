using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    //many-many relationship
    public class AppUserRole:IdentityUserRole<int>
    {
        //join properties 
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
