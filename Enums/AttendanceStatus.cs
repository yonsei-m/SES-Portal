using System.ComponentModel.DataAnnotations;
namespace 社員_求人管理アプリ.Enums;

public enum AttendanceStatus
{
    [Display(Name="通常出勤")]
    Normal = 0,        // 通常出勤
    [Display(Name="有休")]
    PaidLeave = 1,     // 有給
    [Display(Name="欠勤")]
    Absence = 2,       // 欠勤
    [Display(Name="休日出勤")]
    HolidayWork = 3,   // 休日出勤
    [Display(Name="特別休暇")]
    SpecialLeave = 4,  // 特別休暇
    [Display(Name="振替休日")]
    Substitute = 5     // 振替休日
}
