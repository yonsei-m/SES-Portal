namespace 社員_求人管理アプリ.ViewModels.Attendances;

public class TodayAttendanceViewModel
{
    public TimeOnly? ClockIn { get; set; }

    public TimeOnly? ClockOut { get; set; }

    public bool IsWorking =>
        ClockIn.HasValue && !ClockOut.HasValue;

    public bool IsCompleted =>
        ClockIn.HasValue && ClockOut.HasValue;

    public bool NotStarted =>
        !ClockIn.HasValue;

    public string WorkingTime { get; set; } = "";
}