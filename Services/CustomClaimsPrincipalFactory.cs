using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using 社員_求人管理アプリ.Data;

namespace 社員_求人管理アプリ.Services;
public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
{
    private readonly AppDbContext _context;

    public CustomClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        AppDbContext context)
        : base(userManager, roleManager, optionsAccessor)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e =>
                e.UserId == user.Id &&
                !e.IsDeleted);

        if(employee != null)
        {
            identity.AddClaim(
                new Claim(
                    "EmployeeId",
                    employee.Id.ToString()));
        }

        return identity;
    }
}