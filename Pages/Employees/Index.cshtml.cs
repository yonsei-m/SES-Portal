using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SES_Portal.Enums;
using SES_Portal.Models;
using SES_Portal.Services;

namespace SES_Portal.Pages.Employees;

public class IndexModel : PageModel
{
    private readonly EmployeeService _employeeService;
    private readonly DepartmentService _departmentService;
    public IndexModel(
        EmployeeService employeeService, 
        DepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchName { get; set; }
    [BindProperty(SupportsGet = true)]
    public int? SearchDepartmentId { get; set; }
    [BindProperty(SupportsGet = true)]
    public EmployeeStatus? SearchStatus { get; set; }
    [BindProperty(SupportsGet = true)]
    public string? SearchSkill { get; set; }
    // ★追加：ページング
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    [BindProperty(SupportsGet = true)]
    public string? SortColumn { get; set; }
    [BindProperty(SupportsGet = true)]
    public string? SortOrder { get; set; } 
    public List<Department> Departments { get; set; } = new();
    public List<Employee> Employees { get; set; } = new();
    public List<Skill> AllSkills { get; set; } = new();
    [BindProperty(SupportsGet = true)]
    public List<int> SearchSkillIds { get; set; } = new();
    public async Task OnGetAsync()
    {
        Departments = await _departmentService.GetAllAsync();
        AllSkills = await _employeeService.GetActiveSkillsAsync();

        var result = await _employeeService.GetEmployeesAsync(
            SearchName,
            SearchDepartmentId,
            SearchStatus,
            SearchSkillIds,
            SortColumn,
            SortOrder,
            PageNumber,
            PageSize);

        Employees = result.Employees;
        TotalCount = result.TotalCount;
    }
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _employeeService.DeleteAsync(id);
        return RedirectToPage();
    }
    public string GetSortOrder(string column)
    {
        if (SortColumn != column) return "asc";
        return SortOrder == "asc" ? "desc" : "asc";
    }
    public string GetSortIcon(string column)
    {
        if (SortColumn != column) return "↕";
        return SortOrder == "asc" ? "▲" : "▼";
    }
}