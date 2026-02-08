using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Identity
{
    public class ApplicationDbUser:IdentityUser<Guid>
    {
    }
}
