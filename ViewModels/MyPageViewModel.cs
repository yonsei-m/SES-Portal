using SES_Portal.Models;

namespace SES_Portal.ViewModels;

public class MyPageViewModel
{
    public Employee Employee { get; set; } = default!;
    public Attendance? TodayAttendance { get; set; }
    public TimeSpan? WorkingHours { get; set; }
    public List<FavoriteProject> FavoriteProjects { get; set; } = new();
}