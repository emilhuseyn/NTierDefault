using App.Core.Entities.Commons;
using App.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Entities.Identity
{
    public class User : IdentityUser
    {
        public EUserRole EUserRole { get; set; }
    }
}
