using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace RaveCalcApiCommander.Authorization
{
    public class AuthHandler : AuthorizationHandler<AuthRequired>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequired requirement)
        {
            var authParam = _httpContextAccessor.HttpContext.Request.Query["auth"].ToString();

            if (authParam == requirement.SecurityString)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
