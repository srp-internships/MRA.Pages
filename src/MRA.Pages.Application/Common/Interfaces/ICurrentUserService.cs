namespace MRA.Pages.Application.Common.Interfaces;

public interface ICurrentUserService
{
    bool IsSuperAdmin();
    bool IsInRole(string roleName);
    bool IsInRole(IEnumerable<string> roles);
}