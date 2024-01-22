using Microsoft.AspNetCore.Http;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContext) : ICurrentUserService
{
    public bool IsSuperAdmin()
    {
        return httpContext.HttpContext?.User.IsInRole(ApplicationClaimValues.SuperAdministrator) ?? false;
    }
}