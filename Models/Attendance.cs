using System.ComponentModel.DataAnnotations;
using SES_Portal.Enums;

namespace SES_Portal.Models;

public class Attendance
{
    // 勤怠Id
    public int Id { get; set; }
    // 社員id
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    // 勤務日
    [DataType(DataType.Date)]
    public DateTime WorkDate { get; set; }
    // 出勤時間
    public DateTime? ClockIn { get; set; }
    // 退勤時間
    public DateTime? ClockOut { get; set; }
    // 休憩時間
    public int BreakMinutes { get; set; } = 60;
    // 勤怠区分
    public AttendanceStatus Status { get; set; }
    // 作成日時
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}