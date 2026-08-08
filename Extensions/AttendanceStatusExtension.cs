using SES_Portal.Enums;

namespace SES_Portal.Extensions;

public static class AttendanceStatusExtensions
{
    public static string GetShortName(this AttendanceStatus status)
    {
        return status switch
        {
            AttendanceStatus.Normal => "通常",
            AttendanceStatus.PaidLeave => "有休",
            AttendanceStatus.Absence => "欠勤",
            AttendanceStatus.HolidayWork => "休出",
            AttendanceStatus.SpecialLeave => "特休",
            AttendanceStatus.Substitute => "振休",
            _ => ""
        };
    }
}