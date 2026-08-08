namespace SES_Portal.ViewModels;

public class DashboardAnnouncementViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}