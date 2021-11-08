using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Security
{
    public interface IJWTAuthenticationManager
    {
         Task<string> Authenticate(SignInManager<IdentityUser> signInManager, string username, string password, bool rememberMe);
    }
}
