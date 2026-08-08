using Microsoft.EntityFrameworkCore;
using SES_Portal.Data;
using SES_Portal.Models;
using SES_Portal.Enums;
using SES_Portal.ViewModels.Attendances;

namespace SES_Portal.Services;
public class AttendanceService
{
    private readonly AppDbContext _context;
    public AttendanceService(AppDbContext context)
    {
        _context = context;
    }

    // 今日の打刻取得
    public async Task<Attendance?> GetTodayAttendanceAsync(int employeeId)
    {
        var today = DateTime.Today;

        return await _context.Attendances
            .FirstOrDefaultAsync(a =>
                a.EmployeeId == employeeId &&
                a.WorkDate == today);
    }

    // 勤務開始
    public async Task ClockInAsync(int employeeId)
    {
        var attendance = await GetTodayAttendanceAsync(employeeId);

        if (attendance != null)
        {
            // 既に出勤済み
            return;
        }

        attendance = new Attendance
        {
            EmployeeId = employeeId,
            WorkDate = DateTime.Today,
            ClockIn = DateTime.Now,
            BreakMinutes = 0
        };

        _context.Attendances.Add(attendance);

        await _context.SaveChangesAsync();
    }

    // 勤務終了
    public async Task ClockOutAsync(int employeeId)
    {
        var attendance = await GetTodayAttendanceAsync(employeeId);

        if (attendance == null)
        {
            return;
        }
        if (attendance.ClockOut != null)
        {
            // 既に退勤済み
            return;
        }

        attendance.ClockOut = DateTime.Now;
        var workingMinutes =
            (attendance.ClockOut.Value - attendance.ClockIn!.Value)
            .TotalMinutes;

        if (workingMinutes > 8 * 60)
        {
            attendance.BreakMinutes = 60;
        }
        else if (workingMinutes > 6 * 60)
        {
            attendance.BreakMinutes = 45;
        }
        else
        {
            attendance.BreakMinutes = 0;
        }

        await _context.SaveChangesAsync();
    }

    // 勤務時間
    public TimeSpan? GetWorkingHours(Attendance? attendance)
    {
        if(attendance == null)
        {
            return null;
        }

        if(attendance.ClockIn == null ||
        attendance.ClockOut == null)
        {
            return null;
        }

        var minutes = (attendance.ClockOut.Value - attendance.ClockIn.Value).TotalMinutes;

        minutes -= attendance.BreakMinutes;

        if(minutes < 0)
        {
            minutes = 0;
        }

        return TimeSpan.FromMinutes(minutes);
    }
    // 指定月の勤怠一覧
    public async Task<List<Attendance>> GetMonthlyAttendancesAsync(int employeeId, int year, int month)
    {
        return await _context.Attendances
            .Where(a =>
                a.EmployeeId == employeeId &&
                a.WorkDate.Year == year &&
                a.WorkDate.Month == month)
            .OrderBy(a => a.WorkDate)
            .ToListAsync();
    }

    public async Task<List<Attendance>> GetRecentAttendancesAsync(int employeeId, int count = 5)
    {
        return await _context.Attendances
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.WorkDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task UpdateAttendanceAsync(int employeeId, AttendanceEditViewModel model)
    {
        var attendance =
            await _context.Attendances
            .FirstOrDefaultAsync(a =>
                a.EmployeeId == employeeId &&
                a.WorkDate.Date == model.Date.ToDateTime(TimeOnly.MinValue).Date);

        if (attendance == null)
        {
            attendance = new Attendance
            {
                EmployeeId = employeeId,
                WorkDate = model.Date.ToDateTime(TimeOnly.MinValue)
            };

            _context.Attendances.Add(attendance);
        }

        // ClockIn
        if (model.ClockInHour.HasValue && model.ClockInMinute.HasValue)
        {
            attendance.ClockIn = model.Date.ToDateTime(
                new TimeOnly(model.ClockInHour.Value, model.ClockInMinute.Value)
            );
        }
        else
        {
            attendance.ClockIn = null;
        }

        // ClockOut
        if (model.ClockOutHour.HasValue && model.ClockOutMinute.HasValue)
        {
            attendance.ClockOut = model.Date.ToDateTime(
                new TimeOnly(model.ClockOutHour.Value, model.ClockOutMinute.Value)
            );
        }
        else
        {
            attendance.ClockOut = null;
        }

        attendance.BreakMinutes =
            (model.BreakHour ?? 0) * 60 +
            (model.BreakMinute ?? 0);
        attendance.Status = model.Status ?? AttendanceStatus.Normal;

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsWorkingAsync(int employeeId)
    {
        var attendance = await GetTodayAttendanceAsync(employeeId);

        return attendance?.ClockIn != null &&
            attendance.ClockOut == null;
    }   
}