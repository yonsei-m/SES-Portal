using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SES_Portal.Models;
using SES_Portal.ViewModels.Attendances;
using SES_Portal.Services;
using SES_Portal.Enums;

namespace SES_Portal.Pages.Attendances;

public class IndexModel : PageModel
{

    public int DisplayYear { get; set; }
    public int DisplayMonth { get; set; }
    public string DisplayMonthText { get; set; } = "";
    public List<AttendanceCalendarDayViewModel> CalendarDays { get; set; } = [];
    public TodayAttendanceViewModel TodayAttendance { get; set; } = new();
    public List<AttendanceHistoryItemViewModel> RecentAttendances { get; set; } = [];
    public AttendanceSummaryViewModel Summary { get; set; } = new();
    
    private readonly AttendanceService _attendanceService;
    private readonly CurrentUserService _currentUserService;

    public IndexModel(
        AttendanceService attendanceService,
        CurrentUserService currentUserService)
    {
        _attendanceService = attendanceService;
        _currentUserService = currentUserService;
    }
    public async Task OnGetAsync(int? year, int? month)
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);
        if (employee == null)
        {
            return;
        }

        var target = new DateTime(year ?? DateTime.Today.Year, month ?? DateTime.Today.Month, 1);

        DisplayYear = target.Year;
        DisplayMonth = target.Month;
        DisplayMonthText = $"{target.Year}年{target.Month}月";

        var attendances = await _attendanceService
            .GetMonthlyAttendancesAsync(employee.Id, DisplayYear, DisplayMonth);

        var todayAttendance = await _attendanceService.GetTodayAttendanceAsync(employee.Id);
        var recentAttendances = await _attendanceService.GetRecentAttendancesAsync(employee.Id);

        BuildCalendar(target, attendances);

        var completedAttendances = attendances
            .Where(a => a.ClockIn != null &&
                        a.ClockOut != null)
            .ToList();

        var totalMinutes = completedAttendances
            .Sum(a =>
            {
                var working = _attendanceService.GetWorkingHours(a);

                return working?.TotalMinutes ?? 0;
            });
        var overtimeMinutes = completedAttendances
            .Sum(a =>
            {
                var working = _attendanceService.GetWorkingHours(a);
                if (working == null)
                {
                    return 0;
                }
                var overtime = working.Value - TimeSpan.FromHours(8);
                return overtime > TimeSpan.Zero
                    ? overtime.TotalMinutes
                    : 0;
            });

        Summary = new AttendanceSummaryViewModel
        {
            WorkDays = attendances
                .Count(a => a.ClockIn != null),
            TotalWorkingTime = $"{(int)(totalMinutes / 60)}時間{(int)(totalMinutes % 60)}分",
            Overtime = $"{(int)(overtimeMinutes / 60)}時間{(int)(overtimeMinutes % 60)}分",
            PaidLeaveDays = attendances.Count(a => a.Status == AttendanceStatus.PaidLeave)
        };
        
        var workingHours = _attendanceService.GetWorkingHours(todayAttendance);
        TodayAttendance = new TodayAttendanceViewModel
        {
            ClockIn = todayAttendance?.ClockIn != null
                ? TimeOnly.FromDateTime(todayAttendance.ClockIn.Value)
                : null,

            ClockOut = todayAttendance?.ClockOut != null
                ? TimeOnly.FromDateTime(todayAttendance.ClockOut.Value)
                : null,
            WorkingTime = workingHours.HasValue
                ? $"{workingHours.Value.Hours}時間{workingHours.Value.Minutes}分"
                : ""
        };

        RecentAttendances = recentAttendances
            .Select(a =>
            {
                var workingHours = _attendanceService.GetWorkingHours(a);
                return new AttendanceHistoryItemViewModel
                {
                    Date = DateOnly.FromDateTime(a.WorkDate),
                    ClockIn = a.ClockIn.HasValue
                        ? TimeOnly.FromDateTime(a.ClockIn.Value)
                        : null,
                    ClockOut = a.ClockOut.HasValue
                        ? TimeOnly.FromDateTime(a.ClockOut.Value)
                        : null,
                    WorkingTime = workingHours.HasValue
                        ? $"{(int)workingHours.Value.TotalHours}時間{workingHours.Value.Minutes}分"
                        : "",
                    Status = null // 後で勤怠区分を実装する場合に置き換える
                };
            })
            .ToList();
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
    private void BuildCalendar(DateTime target, List<Attendance> attendances)
    {
        CalendarDays.Clear();

        var firstDay = new DateTime(target.Year, target.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);
        int offset = (int)firstDay.DayOfWeek;

        for (int i = 0; i < offset; i++)
        {
            CalendarDays.Add(new AttendanceCalendarDayViewModel());
        }
        for (int day = 1; day <= lastDay.Day; day++)
        {
            var date = new DateTime(target.Year, target.Month, day);
            var attendanceMap = attendances
                .ToDictionary(
                    a => a.WorkDate.Date
                );
            attendanceMap.TryGetValue(date.Date, out var attendance);
            CalendarDays.Add(new AttendanceCalendarDayViewModel
            {
                Date = date,
                IsToday = date == DateTime.Today,
                ClockIn = attendance?.ClockIn != null
                    ? TimeOnly.FromDateTime(attendance.ClockIn.Value)
                    : null,
                ClockOut = attendance?.ClockOut != null
                    ? TimeOnly.FromDateTime(attendance.ClockOut.Value)
                    : null,
                Status = attendance?.Status,
                BreakMinutes = attendance?.BreakMinutes ?? 0
            });
        }
        while (CalendarDays.Count % 7 != 0)
        {
            CalendarDays.Add(new AttendanceCalendarDayViewModel());
        }
    }

        public async Task<IActionResult> OnPostEditAsync(AttendanceEditViewModel model)
    {
        var employee = await _currentUserService.GetCurrentEmployeeAsync(User);

        if (employee == null)
        {
            return RedirectToPage();
        }
    
        await _attendanceService.UpdateAttendanceAsync(employee.Id, model);
        return RedirectToPage();
    }
}