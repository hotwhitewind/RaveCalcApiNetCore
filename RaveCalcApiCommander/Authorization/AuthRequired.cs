using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Authorization
{
    public class AuthRequired : IAuthorizationRequirement
    {
        public string SecurityString { get; set; }
        public AuthRequired(string securityString)
        {
            SecurityString = securityString;
        }
    }
}
