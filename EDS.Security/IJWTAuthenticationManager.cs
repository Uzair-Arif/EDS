using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Security
{
    public interface IJWTAuthenticationManager
    {
         Task<string> Authenticate(string username, string password, bool rememberMe);
    }
}
