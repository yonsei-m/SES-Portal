using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.ViewModels;

public class DashboardViewModel
{
    // 公開中プロジェクト
    public int OpenProjectCount { get; set; }
    // 自分のお気に入り案件数
    public int FavoriteProjectCount { get; set; }

    public List<Project> LatestProjects { get; set; } = new();

    public List<DashboardAnnouncementViewModel> LatestAnnouncements { get; set; } = new();
}