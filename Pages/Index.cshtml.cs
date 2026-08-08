using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using 社員_求人管理アプリ.Services;
using 社員_求人管理アプリ.ViewModels;
using 社員_求人管理アプリ.Models;
namespace 社員_求人管理アプリ.Pages;


[Authorize]public class IndexModel : PageModel
{
    private readonly DashboardService _dashboardService;
    private readonly AttendanceService _attendanceService;
    private readonly CurrentUserService _currentUserService;

    public DashboardViewModel Dashboard { get; set; } = new();
    public Attendance? TodayAttendance { get; set; }
    public TimeSpan? WorkingHours { get; set; }

    public IndexModel(
        DashboardService dashboardService,
        AttendanceService attendanceService,
        CurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _attendanceService = attendanceService;
        _currentUserService = currentUserService;
    }
    public async Task OnGetAsync()
    {
        Dashboard.OpenProjectCount = await _dashboardService.GetOpenProjectCountAsync();
        Dashboard.LatestProjects = await _dashboardService.GetLatestProjectsAsync();
        Dashboard.LatestAnnouncements = await _dashboardService.GetLatestAnnouncementsAsync();
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee != null)
        {
            Dashboard.FavoriteProjectCount =
                await _dashboardService.GetFavoriteProjectCountAsync(employee.Id);

            TodayAttendance =
                await _attendanceService.GetTodayAttendanceAsync(employee.Id);

            WorkingHours =
                _attendanceService.GetWorkingHours(TodayAttendance);
        }
    }
    public async Task<IActionResult> OnPostClockInAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
            return RedirectToPage();
        await _attendanceService.ClockInAsync(employee.Id);
        return RedirectToPage();
    }
    public async Task<IActionResult> OnPostClockOutAsync()
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
            return RedirectToPage();
        await _attendanceService.ClockOutAsync(employee.Id);
        return RedirectToPage();
    }
}