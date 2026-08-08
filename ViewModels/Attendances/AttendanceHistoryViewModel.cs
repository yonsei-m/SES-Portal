namespace 社員_求人管理アプリ.ViewModels.Attendances;

public class AttendanceHistoryItemViewModel
{
    public DateOnly Date { get; set; }

    public TimeOnly? ClockIn { get; set; }

    public TimeOnly? ClockOut { get; set; }

    public string WorkingTime { get; set; } = "";

    public string? Status { get; set; }
}