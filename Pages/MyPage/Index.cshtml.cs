using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using 社員_求人管理アプリ.Services;
using 社員_求人管理アプリ.ViewModels;

namespace 社員_求人管理アプリ.Pages.MyPage;


[Authorize]
public class IndexModel : PageModel
{
    private readonly EmployeeService _employeeService;
    private readonly AttendanceService _attendanceService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly FavoriteProjectService _favoriteProjectService;
    private readonly CurrentUserService _currentUserService;

    public IndexModel(
        EmployeeService employeeService,
        AttendanceService attendanceService,
        UserManager<IdentityUser> userManager,
        FavoriteProjectService favoriteProjectService,
        CurrentUserService currentUserService)
    {
        _employeeService = employeeService;
        _attendanceService = attendanceService;
        _userManager = userManager;
        _favoriteProjectService = favoriteProjectService;
        _currentUserService = currentUserService;
    }

    public MyPageViewModel MyPage { get; private set; } = new();
    public async Task OnGetAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
        {
            return;
        }

        var attendance =
            await _attendanceService.GetTodayAttendanceAsync(employee.Id);

        MyPage.Employee = employee;

        MyPage.TodayAttendance = attendance;

        MyPage.WorkingHours =
            _attendanceService.GetWorkingHours(attendance);
        
        MyPage.FavoriteProjects =
            await _favoriteProjectService
            .GetFavoritesByEmployeeAsync(employee.Id);
    }
    public async Task<IActionResult> OnPostClockInAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
        {
            return RedirectToPage();
        }

        await _attendanceService.ClockInAsync(employee.Id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostClockOutAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
        {
            return RedirectToPage();
        }

        await _attendanceService.ClockOutAsync(employee.Id);

        return RedirectToPage();
    }
}