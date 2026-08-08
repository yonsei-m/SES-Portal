using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SES_Portal.Models;

namespace SES_Portal.Services;
public class CurrentUserService
{
    private readonly EmployeeService _employeeService;

    public CurrentUserService(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<Employee?> GetCurrentEmployeeAsync(ClaimsPrincipal user)
    {
        var employeeId = user.FindFirst("EmployeeId")?.Value;

        if (!int.TryParse(employeeId, out var id))
        {
            return null;
        }

        return await _employeeService.GetByIdAsync(id);
    }
}