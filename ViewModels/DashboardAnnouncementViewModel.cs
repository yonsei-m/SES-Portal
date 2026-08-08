namespace 社員_求人管理アプリ.ViewModels;

public class DashboardAnnouncementViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}