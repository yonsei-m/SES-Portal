namespace SES_Portal.Models;

public class AnnouncementRead
{
    // お知らせ既読Id
    public int Id { get; set; }
    // お知らせId
    public int AnnouncementId { get; set; }
    public Announcement Announcement { get; set; } = null!;
    // 社員Id
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    // 既読時刻
    public DateTime ReadAt { get; set; } = DateTime.Now;
}