using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SES_Portal.Models;
using SES_Portal.Services;
using SES_Portal.Data;

namespace SES_Portal.Pages.Employees;

public class DetailsModel : PageModel
{
    private readonly EmployeeService _employeeService;
    private readonly ImageService _imageService;
    private readonly CurrentUserService _currentUserService;
    public DetailsModel(
        EmployeeService employeeService,
        ImageService imageService,
        CurrentUserService currentUserService)
    {
        _employeeService = employeeService;
        _imageService = imageService;
        _currentUserService = currentUserService;
    }
    [BindProperty]
    public Employee? Employee { get; set; } = new();
    public bool CanEdit { get; private set; }
    [BindProperty]
    public IFormFile? ImageFile { get; set; }
    public List<Skill> AllSkills { get; set; } = new();
    [BindProperty]
    public List<int> SelectedSkillIds { get; set; } = new();
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await LoadEmployeeAsync(id);
        if(result is NotFoundResult)
        {
            return result;
        }
        AllSkills = await _employeeService.GetActiveSkillsAsync();
        SelectedSkillIds = await _employeeService.GetEmployeeSkillIdsAsync(Employee!.Id);
        return result;
    }
    public async Task<IActionResult> OnPostAsync(int id)
    {Console.WriteLine($"自己紹介:{Employee?.SelfIntroduction}");
Console.WriteLine($"経験年数:{Employee?.ExperienceYears}");
        if (Employee == null)
        {
            return BadRequest();
        }
        var currentEmployee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if(currentEmployee == null)
        {
            return Unauthorized();
        }

        var employee = await _employeeService.GetByIdAsync(id);
        if(employee == null || employee.Id != currentEmployee.Id)
        {
            return Forbid();
        }

        employee.ExperienceYears = Employee.ExperienceYears;
        employee.SelfIntroduction = Employee.SelfIntroduction;

        if(ImageFile != null)
        {
            employee.ImagePath =
                await _imageService
                .SaveEmployeeImageAsync(ImageFile);
        }

        await _employeeService.UpdateEmployeeSkillsAsync(employee.Id, SelectedSkillIds);
        await _employeeService.UpdateAsync(employee);
        return RedirectToPage(new { id });
    }
    private async Task<IActionResult> LoadEmployeeAsync(int id)
    {
        Employee = await _employeeService.GetByIdAsync(id);
        if(Employee == null)
        {
            return NotFound();
        }
        var currentEmployee =
            await _currentUserService.GetCurrentEmployeeAsync(User);

        if(currentEmployee != null)
        {
            CanEdit = currentEmployee.Id == Employee.Id;
        }

        return Page();
    }
}