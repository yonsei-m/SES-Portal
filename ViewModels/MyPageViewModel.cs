using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.ViewModels;

public class MyPageViewModel
{
    public Employee Employee { get; set; } = default!;
    public Attendance? TodayAttendance { get; set; }
    public TimeSpan? WorkingHours { get; set; }
    public List<FavoriteProject> FavoriteProjects { get; set; } = new();
}