namespace 社員_求人管理アプリ.ViewModels.Attendances;

public class AttendanceSummaryViewModel
{
    public int WorkDays { get; set; }

    public string TotalWorkingTime { get; set; } = "";

    public string Overtime { get; set; } = "";

    public int PaidLeaveDays { get; set; }
}