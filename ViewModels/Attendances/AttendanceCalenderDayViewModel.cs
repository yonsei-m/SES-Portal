using SES_Portal.Enums;
namespace SES_Portal.ViewModels.Attendances;

public class AttendanceCalendarDayViewModel
{
    public DateTime? Date { get; set; }

    public bool IsToday { get; set; }

    public bool IsCurrentMonth => Date.HasValue;

    public TimeOnly? ClockIn { get; set; }

    public TimeOnly? ClockOut { get; set; }
    public int BreakMinutes { get; set; } = 60;
    public DayOfWeek DayOfWeek => Date?.DayOfWeek ?? DayOfWeek.Sunday;

    public bool IsWeekend =>
        Date?.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public bool HasAttendance =>
        ClockIn.HasValue || ClockOut.HasValue;

    public AttendanceStatus? Status { get; set; }
    
}