using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppRole:IdentityRole<int>
    {
        //Create colleciton of AppUserRole
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
