using SES_Portal.Enums;

namespace SES_Portal.ViewModels.Attendances;

public class AttendanceEditViewModel
{
    public DateOnly Date { get; set; }

    public int? ClockInHour { get; set; }
    public int? ClockInMinute { get; set; }

    public int? ClockOutHour { get; set; }
    public int? ClockOutMinute { get; set; }

    public int? BreakHour { get; set; }
    public int? BreakMinute { get; set; }

    public AttendanceStatus? Status { get; set; }
}