using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Attributes;

public sealed class MvcAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    internal static string LoginUrl { get; set; } = "";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity == null ||
            !context.HttpContext.User.IsInRole(ApplicationClaimValues.SuperAdministrator))
        {
            context.Result = new RedirectResult(LoginUrl);
        }
    }
}