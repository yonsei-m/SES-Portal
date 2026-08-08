namespace SES_Portal.ViewModels.Attendances;

public class AttendanceSummaryViewModel
{
    public int WorkDays { get; set; }

    public string TotalWorkingTime { get; set; } = "";

    public string Overtime { get; set; } = "";

    public int PaidLeaveDays { get; set; }
}